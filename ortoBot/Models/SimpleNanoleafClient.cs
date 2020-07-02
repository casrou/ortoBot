using Nanoleaf.Client;
using Nanoleaf.Client.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ortoBot
{
    public class SimpleNanoleafClient : NanoleafClient
    {
        public SimpleNanoleafClient(string host, string userToken) : base(host, userToken) { }

        public new async Task<string> GetCurrentEffectAsync()
        {
            string result = await base.GetCurrentEffectAsync();
            Log.Information($"Nanoleaf: GetCurrentEffectAsync() -> {result}");
            return result;
        }

        public new async Task<List<string>> GetEffectsAsync()
        {
            List<string> result = await base.GetEffectsAsync();
            Log.Information($"Nanoleaf: GetEffectsAsync({String.Join(", ", result)})");
            return result;
        }

        public new async Task SetEffectAsync(string effectName)
        {
            try
            {
                await base.SetEffectAsync(effectName);
                Log.Information($"Nanoleaf: SetEffectAsync({effectName})");
            }
            catch (NanoleafHttpException e)
            {
                Log.Fatal(e.ToString());
                throw;
            }
        }
    }
}
