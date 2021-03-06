﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Timers;
using AquaLightControl.Gui.Helper;
using AquaLightControl.Gui.Model;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using ReactiveUI;

namespace AquaLightControl.Gui.ViewModels.Controls
{
    public sealed class LightConfigurationViewModel : ReactiveObject, IDisposable
    {
        private static readonly long _ticks_per_day = TimeSpan.FromHours(24).Ticks;
        private readonly List<LedLightCurveModel> _curve_models;
        private readonly PlotModel _model;
        private readonly IPlotController _controller;
        private readonly LineAnnotation _now_annotation;

        private ObservableCollection<LedDeviceModel> _led_devices;
        private LedDeviceModel _selected_led_device;
        private bool _show_only_selected_device;
        private bool _has_modified_items;
        private IDisposable _led_devices_changed_disposable;
        private IDisposable _modified_curve_models_checker;
        private bool _show_tool_tips;
        private IDisposable _timer;

        public LightConfigurationViewModel() {
            _curve_models = new List<LedLightCurveModel>();
            _model = CreateNewPlotModel();
            _controller = CreateNewController();

            _now_annotation = CreateLineAnnotation();
            _model.Annotations.Add(_now_annotation);

            InitializeLightTimer();
        }

        private void InitializeLightTimer() {
            _timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), DispatcherScheduler.Current)
                .Subscribe(_ => SetCurrentTimeLine());
        }

        public LedDeviceModel SelectedLedDevice {
            get { return _selected_led_device; }
            set { 
                this.RaiseAndSetIfChanged(ref _selected_led_device, value);
                UpdatePlotModel();
            }
        }

        public bool ShowOnlySelectedDevice {
            get { return _show_only_selected_device; }
            set { 
                this.RaiseAndSetIfChanged(ref _show_only_selected_device, value);
                UpdatePlotModel();
            }
        }

        public bool HasModifiedItems {
            get { return _has_modified_items; }
            set { this.RaiseAndSetIfChanged(ref _has_modified_items,  value); }
        }

        public PlotModel Model {
            get { return _model; }
        }

        public IPlotController Controller {
            get { return _controller; }
        }

        public ObservableCollection<LedDeviceModel> LedDevices {
            get { return _led_devices; }
            set { 
                this.RaiseAndSetIfChanged(ref _led_devices, value);

                InitializeModels();
            }
        }
        public bool ShowToolTips {
            get { return _show_tool_tips; }
            set {
                SetShowToolTips(value);
                this.RaiseAndSetIfChanged(ref _show_tool_tips, value); 
            }
        }

        private void InitializeModels() {
            UnsubscribeLedDeviceChanges();

            var collection = _led_devices;
            
            RecreateCurveModels(collection);
            UpdatePlotModel();
            
            if (!ReferenceEquals(collection, null)) {
                var changed_items = Observable
                    .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        o => collection.CollectionChanged += o,
                        o => collection.CollectionChanged -= o);

                _led_devices_changed_disposable = changed_items.Subscribe(OnLedDevicesChanged);
            } 
        }

        private void SetCurrentTimeLine() {
            _now_annotation.X = Math.Min(DateTime.Now.TimeOfDay.Ticks, _ticks_per_day).ConvertX();
            _model.InvalidatePlot(false);
        }

        private void RecreateCurveModels(IEnumerable<LedDeviceModel> collection) {
            ClearCurveModels();
            
            if (ReferenceEquals(collection, null)) {
                return;
            }

            AddCurveModels(collection.Select(CreateCurveModel));
        }

        private void SetShowToolTips(bool value) {
            if (value) {
                _controller.BindMouseEnter(PlotCommands.HoverTrack);
            } else {
                _controller.UnbindMouseEnter();
            }
            _model.InvalidatePlot(false);
        }

        private static PlotModel CreateNewPlotModel() {
            var plot_model = new PlotModel {
                LegendSymbolLength = 24,
                Title = "Lichtzeiten"
            };

            var bottom_axis = new TimeSpanAxis {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Minimum = DateTimeCalculator.GetMinimum(),
                AbsoluteMinimum = DateTimeCalculator.GetMinimum(),
                Maximum = DateTimeCalculator.GetMaximum(),
                AbsoluteMaximum = DateTimeCalculator.GetMaximum(),
            };

            var left_axis = new LinearAxis {
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left,
                Minimum = 0,
                AbsoluteMinimum = 0,
                Maximum = 65535,
                AbsoluteMaximum = 65536,
                IsZoomEnabled = false
            };
            
            plot_model.Axes.Add(bottom_axis);
            plot_model.Axes.Add(left_axis);
            return plot_model;
        }

        private static IPlotController CreateNewController() {
            return new PlotController();
        }

        private static LineAnnotation CreateLineAnnotation() {
            var now_annotation = new LineAnnotation {
                Type = LineAnnotationType.Vertical,
                X = DateTimeCalculator.GetMinimum(),
                LineStyle = LineStyle.Solid
            };

            return now_annotation;
        }

        private void UnsubscribeLedDeviceChanges() {
            var disposable = _led_devices_changed_disposable;
            if (ReferenceEquals(disposable, null)) {
                return;
            }
            
            disposable.Dispose();
            _led_devices_changed_disposable = null;
        }

        private void OnLedDevicesChanged(EventPattern<NotifyCollectionChangedEventArgs> @event) {
            var args = @event.EventArgs;
            switch (args.Action) {
                case NotifyCollectionChangedAction.Add:
                    if (!ReferenceEquals(args.NewItems, null)) {
                        var new_curve_models = args.NewItems.Cast<LedDeviceModel>().Select(CreateCurveModel);
                        AddCurveModels(new_curve_models);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (!ReferenceEquals(args.OldItems, null)) {
                        var items_to_remove = args.OldItems
                            .Cast<LedDeviceModel>()
                            .Select(m => m.Id)
                            .ToArray();

                        RemoveCurveModels(model => items_to_remove.Contains(model.Id));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (!ReferenceEquals(args.OldItems, null)) {
                        var items_to_replace = args.OldItems
                            .Cast<LedDeviceModel>()
                            .Select(m => m.Id)
                            .ToArray();

                        RemoveCurveModels(model => items_to_replace.Contains(model.Id));
                    }
                    if (!ReferenceEquals(args.NewItems, null)) {
                        var new_curve_models = args.NewItems.Cast<LedDeviceModel>().Select(CreateCurveModel);
                        AddCurveModels(new_curve_models);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearCurveModels();
                    if (!ReferenceEquals(args.NewItems, null)) {
                        var new_curve_models = args.NewItems.Cast<LedDeviceModel>().Select(CreateCurveModel);
                        AddCurveModels(new_curve_models);
                    }
                    break;
            }

            UpdatePlotModel();
        }

        private void RemoveCurveModels(Func<LedLightCurveModel, bool> remove_predicate) {
            var items_to_remove = _curve_models
                .Where(remove_predicate)
                .ToArray();

            items_to_remove.ForEach(m => {
                m.Dispose();
                _curve_models.Remove(m);
            });

            UpdateModifiedItemCheck();
        }

        private void AddCurveModels(IEnumerable<LedLightCurveModel> new_curve_models) {
            _curve_models.AddRange(new_curve_models);
            
            UpdateModifiedItemCheck();
        }

        private void StopTimer() {
            if (ReferenceEquals(_timer, null)) {
                return;
            }

            _timer.Dispose();
        }

        private void ClearCurveModels() {
            _curve_models.ForEach(m => m.Dispose());
            _curve_models.Clear();
            
            UpdateModifiedItemCheck();
        }

        private void UpdateModifiedItemCheck() {
            var checker = _modified_curve_models_checker;
            if (!ReferenceEquals(checker, null)) {
                checker.Dispose();
            }
            
            checker = _curve_models
                .Select(cm => cm.WhenAny(m => m.IsModified, s => s.Value))
                .Merge()
                .Subscribe(arg => { HasModifiedItems = _curve_models.Any(cm => cm.IsModified); });

            _modified_curve_models_checker = checker;
        }

        private void UpdatePlotModel() {
            var plot_model = Model;
            if (ReferenceEquals(plot_model, null)) {
                return;
            }

            plot_model.Series.Clear();

            if (!_show_only_selected_device) {
                _curve_models.ForEach(m => plot_model.Series.Add(m.Line));
                raisePropertyChanged("Model");
                plot_model.InvalidatePlot(false);
                return;
            }
            var selected_led_device = _selected_led_device;
            if (ReferenceEquals(selected_led_device, null)) {
                plot_model.InvalidatePlot(false);
                return;
            }

            var curve_model = _curve_models.FirstOrDefault(m => m.Id == selected_led_device.Id);
            if (ReferenceEquals(curve_model, null)) {
                plot_model.InvalidatePlot(false);
                return;
            }

            plot_model.Series.Add(curve_model.Line);
            plot_model.InvalidatePlot(false);
            raisePropertyChanged("Model");
        }

        public void Update() {
            _curve_models.ForEach(m => m.Update());
        }

        public void Dispose() {
            ClearCurveModels();
            UnsubscribeLedDeviceChanges();
            StopTimer();
        }

        private static LedLightCurveModel CreateCurveModel(LedDeviceModel led_device_model) {
            return new LedLightCurveModel(led_device_model);
        }
    }
}