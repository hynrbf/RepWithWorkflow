using System.ComponentModel;

namespace Common.Entities
{
    public enum FitchRating
    {
        NotRated,
        AAA,
        [Description("AA+")] AA_Plus,
        AA,
        [Description("AA-")] AA_Minus,
        [Description("A+")] A_Plus,
        A,
        [Description("A-")] A_Minus,
        [Description("BBB+")] BBB_Plus,
        BBB,
        [Description("BBB-")] BBB_Minus,
        [Description("BB+")] BB_Plus,
        BB,
        [Description("BB-")] BB_Minus,
        [Description("B+")] B_Plus,
        B,
        [Description("B-")] B_Minus,
        CCC,
        CC,
        C
    }
}