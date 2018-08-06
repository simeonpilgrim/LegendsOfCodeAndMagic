using System.Collections.Generic;
using System.Text;

namespace LOCAM
{
    /**
     * http://dominisz.pl
     * 02.04.2018
     */
    public class Keywords
    {

        public bool hasBreakthrough;
        public bool hasCharge;
        public bool hasDrain;
        public bool hasGuard;
        public bool hasLethal;
        //public bool hasRegenerate;
        public bool hasWard;

        public bool hasAnyKeyword()
        {
            return hasBreakthrough || hasCharge || hasDrain || hasGuard || hasLethal /*|| hasRegenerate*/ || hasWard;
        }

        //TODO maybe this method should return already joined string
        public List<string> getListOfKeywords()
        {
            List<string> keywords = new List<string>();
            if (hasBreakthrough) keywords.Add("Breakthrough");
            if (hasCharge) keywords.Add("Charge");
            if (hasDrain) keywords.Add("Drain");
            if (hasGuard) keywords.Add("Guard");
            if (hasLethal) keywords.Add("Lethal");
            //if (hasRegenerate) keywords.Add("Regenerate");
            if (hasWard) keywords.Add("Ward");
            return keywords;
        }

        public Keywords(string data)
        {
            hasBreakthrough = data[0] == 'B';
            hasCharge = data[1] == 'C';
            hasDrain = data[2] == 'D';
            hasGuard = data[3] == 'G';
            hasLethal = data[4] == 'L';
            //hasRegenerate = data[5] == 'R';
            hasWard = data[5] == 'W';
        }

        public Keywords(Keywords keywords)
        {
            hasBreakthrough = keywords.hasBreakthrough;
            hasCharge = keywords.hasCharge;
            hasDrain = keywords.hasDrain;
            hasGuard = keywords.hasGuard;
            hasLethal = keywords.hasLethal;
            //hasRegenerate = keywords.hasRegenerate;
            hasWard = keywords.hasWard;
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(hasBreakthrough ? 'B' : '-');
            sb.Append(hasCharge ? 'C' : '-');
            sb.Append(hasDrain ? 'D' : '-');
            sb.Append(hasGuard ? 'G' : '-');
            sb.Append(hasLethal ? 'L' : '-');
            //sb.Append(hasRegenerate ? 'R' : '-');
            sb.Append(hasWard ? 'W' : '-');
            return sb.ToString();
        }
    }
}