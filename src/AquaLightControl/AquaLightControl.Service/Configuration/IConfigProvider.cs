﻿namespace AquaLightControl.Service.Configuration
{
    public interface IConfigProvider
    {
        string GetKey(string key_name);
    }
}