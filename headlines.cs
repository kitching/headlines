using System.Diagnostics;

class Headlines
{    
    static void Main(string[] args)
    {
        var h = new Headlines();
        h.Load();

        var tests = new List<(string headline, string[] tickers)>
        {
            ("things are looking up say rio tinto", ["RIO"]),
            ("Glencore reports Q1 earnings", ["GLEN"]),
            ("now rio tinto reports Q1 earnings", ["RIO"]),
            ("business as usual for anglo american plc", ["AAL"]),
            ("mining trending up for rio tinto and anglo american plc", ["AAL", "RIO"]),
            ("xxxxx", []),
            ("", [])
        };       

        foreach (var t in tests)
        {
            var result = h.GetTickers(t.headline);
            Console.WriteLine($"{t.headline} = {string.Join(",", result)}");
            Debug.Assert(result.Order().SequenceEqual(t.tickers.Order()));
        }
    }

    Dictionary<string, string> namesToTickers { get; set; }
    int maxWordsInName = 0;

    public void Load()
    {
        namesToTickers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"3i","III"},
            {"Admiral Group","ADM"},
            {"Airtel Africa","AAF"},
            {"Anglo American plc","AAL"},
            {"Antofagasta plc","ANTO"},
            {"Ashtead Group","AHT"},
            {"Associated British Foods","ABF"},
            {"AstraZeneca","AZN"},
            {"Auto Trader Group","AUTO"},
            {"Aviva","AV"},
            {"B&M","BME"},
            {"BAE Systems","BA"},
            {"Barclays","BARC"},
            {"Barratt Redrow","BTRW"},
            {"Beazley","BEZ"},
            {"Berkeley Group Holdings","BKG"},
            {"BP","BP"},
            {"British American Tobacco","BATS"},
            {"British Land","BLND"},
            {"BT Group","BT-A"},
            {"Bunzl","BNZL"},
            {"Centrica","CNA"},
            {"Coca-Cola HBC","CCH"},
            {"Compass Group","CPG"},
            {"Convatec","CTEC"},
            {"Croda International","CRDA"},
            {"DCC plc","DCC"},
            {"Diageo","DGE"},
            {"Diploma","DPLM"},
            {"Endeavour Mining","EDV"},
            {"Entain","ENT"},
            {"EasyJet","EZJ"},
            {"Experian","EXPN"},
            {"F & C Investment Trust","FCIT"},
            {"Frasers Group","FRAS"},
            {"Fresnillo plc","FRES"},
            {"Glencore","GLEN"},
            {"GSK plc","GSK"},
            {"Haleon","HLN"},
            {"Halma plc","HLMA"},
            {"Hargreaves Lansdown","HL"},
            {"Hikma Pharmaceuticals","HIK"},
            {"Hiscox","HSX"},
            {"Howdens Joinery","HWDN"},
            {"HSBC","HSBA"},
            {"IHG Hotels & Resorts","IHG"},
            {"IMI","IMI"},
            {"Imperial Brands","IMB"},
            {"Informa","INF"},
            {"Intermediate Capital Group","ICG"},
            {"International Airlines Group","IAG"},
            {"Intertek","ITRK"},
            {"JD Sports","JD"},
            {"Kingfisher plc","KGF"},
            {"Land Securities","LAND"},
            {"Legal & General","LGEN"},
            {"Lloyds Banking Group","LLOY"},
            {"LondonMetric Property","LMP"},
            {"London Stock Exchange Group","LSEG"},
            {"M&G","MNG"},
            {"Marks & Spencer","MKS"},
            {"Melrose Industries","MRO"},
            {"Mondi","MNDI"},
            {"National Grid plc","NG"},
            {"NatWest Group","NWG"},
            {"Next plc","NXT"},
            {"Pearson plc","PSON"},
            {"Pershing Square Holdings","PSH"},
            {"Persimmon","PSN"},
            {"Phoenix Group","PHNX"},
            {"Prudential plc","PRU"},
            {"Reckitt","RKT"},
            {"RELX","REL"},
            {"Rentokil Initial","RTO"},
            {"Rightmove","RMV"},
            {"Rio Tinto","RIO"},
            {"Rolls-Royce Holdings","RR"},
            {"Sage Group","SGE"},
            {"Sainsbury's","SBRY"},
            {"Schroders","SDR"},
            {"Scottish Mortgage Investment Trust","SMT"},
            {"Segro","SGRO"},
            {"Severn Trent","SVT"},
            {"Shell plc","SHEL"},
            {"DS Smith","SMDS"},
            {"Smiths Group","SMIN"},
            {"Smith & Nephew","SN"},
            {"Spirax Group","SPX"},
            {"SSE plc","SSE"},
            {"Standard Chartered","STAN"},
            {"Taylor Wimpey","TW"},
            {"Tesco","TSCO"},
            {"Unilever","ULVR"},
            {"United Utilities","UU"},
            {"Unite Group","UTG"},
            {"Vistry Group","VTY"},
            {"Vodafone Group","VOD"},
            {"Weir Group","WEIR"},
            {"Whitbread","WTB"},
            {"WPP plc","WPP"}
        };

        maxWordsInName = namesToTickers.Keys.Max(x => x.Split(" ").Length);
    }

    public IEnumerable<string> GetTickers(string newsHeadline)
    {
        var tickers = new HashSet<string>();

        string[] tokens = newsHeadline.Split(" "); // O(N). might be faster using ReadOnlySpan<char>.Split

        // chunk the headline into all possible n-grams (substrings of n whitespace delimited words) of all lengths 1 to min(maxWordsInName,words)

        var ngrams = new HashSet<string>(tokens, StringComparer.OrdinalIgnoreCase);

        // add all chunks to set, O(1) each time
        for (int ngramLength = 2; ngramLength <= Math.Min(maxWordsInName, tokens.Length); ngramLength++)
        {
            for (int i = 0; i <= tokens.Length-ngramLength; i++)
            {
                Range r = new Range(i, i+ngramLength);
                ngrams.Add(string.Join(" ", tokens[r]));
            }
        }

        foreach (string ng in ngrams)
        {
            // if ngram exists in Dictionary<name,ticker> then add associated ticker to result, O(1) * ngrams
            if (namesToTickers.TryGetValue(ng, out string ticker))
                tickers.Add(ticker);
        }

        return tickers;
    }
}
