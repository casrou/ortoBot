using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanoleaf.Client.Interfaces;
using Nanoleaf.Client.Models.Responses;
using Serilog;

namespace ortoBot
{
    internal class FakeNanoleafClient : INanoleafClient
    {
        private string currentEffect;
        private Settings settings;

        public FakeNanoleafClient(Settings settings)
        {
            this.settings = settings;
        }

        public string GetCurrentEffect()
        {
            Log.Information($"DEBUG: Nanoleaf: GetCurrentEffectAsync() -> {currentEffect}");
            return currentEffect;
        }

        public Task<List<string>> GetEffectsAsync()
        {
            List<string> result = settings.keywords.Values.ToList();
            Log.Information($"DEBUG: Nanoleaf: GetEffectsAsync({String.Join(", ", result)})");
            return Task.FromResult(result);
        }

        public async Task SetEffectAsync(string effectName)
        {
            currentEffect = effectName;
            Log.Information($"DEBUG: Nanoleaf: SetEffectAsync({effectName})");
            await Task.Delay(0);
        }

        public Task<int> GetBrightnessAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Brightness> GetBrightnessInfoAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetBrightnessMaxValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetBrightnessMinValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetColorModeAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetColorTemperatureAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetColorTemperatureMaxValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetColorTemperatureMinValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetCurrentEffectAsync()
        {
            throw new System.NotImplementedException();
        }
        
        public Task<int> GetHueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Hue> GetHueInfoAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetHueMaxValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetHueMinValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Info> GetInfoAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> GetPowerStatusAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetSaturationAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Saturation> GetSaturationInfoAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetSaturationMaxValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetSaturationMinValueAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ColorTemperature> GetTemperatureInfoAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task LowerBrightnessAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task LowerColorTemperatureAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task LowerHueAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task LowerSaturationAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task RaiseBrightnessAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task RaiseColorTemperatureAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task RaiseHueAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task RaiseSaturationAsync(int value)
        {
            throw new System.NotImplementedException();
        }

        public Task SetBrightnessAsync(int targetBrightness, int time = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task SetColorTemperatureAsync(int targetCt)
        {
            throw new System.NotImplementedException();
        }
        

        public Task SetHueAsync(int targetHue)
        {
            throw new System.NotImplementedException();
        }

        public Task SetSaturationAsync(int targetSat)
        {
            throw new System.NotImplementedException();
        }

        public Task TurnOffAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task TurnOnAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<UserToken> CreateTokenAsync()
        {
            throw new NotImplementedException();
        }

        public void Authorize(string token)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTokenAsync(string userToken)
        {
            throw new NotImplementedException();
        }

        public Task SetHsvAsync(int h, int s, int v)
        {
            throw new NotImplementedException();
        }

        public Task SetRgbAsync(int r, int g, int b)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}