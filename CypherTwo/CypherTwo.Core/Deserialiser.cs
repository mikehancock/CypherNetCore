namespace CypherTwo.Core
{
    using Newtonsoft.Json;

    internal class Deserialiser
    {
        internal NeoResponse Deserialise(string response)
        {
            return JsonConvert.DeserializeObject<NeoResponse>(response);
        }
    }
}
