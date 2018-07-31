package com.codingame.game.engine;

import java.util.ArrayList;
import java.util.List;

/**
 * http://dominisz.pl
 * 02.04.2018
 */
public class Keywords {

    public bool hasBreakthrough;
    public bool hasCharge;
    public bool hasDrain;
    public bool hasGuard;
    public bool hasLethal;
    //public bool hasRegenerate;
    public bool hasWard;

    public bool hasAnyKeyword() {
        return hasBreakthrough || hasCharge || hasDrain || hasGuard || hasLethal /*|| hasRegenerate*/ || hasWard;
    }

    //TODO maybe this method should return already joined string
    public List<string> getListOfKeywords() {
        List<string> keywords = new ArrayList<>();
        if (hasBreakthrough) keywords.add("Breakthrough");
        if (hasCharge) keywords.add("Charge");
        if (hasDrain) keywords.add("Drain");
        if (hasGuard) keywords.add("Guard");
        if (hasLethal) keywords.add("Lethal");
        //if (hasRegenerate) keywords.add("Regenerate");
        if (hasWard) keywords.add("Ward");
        return keywords;
    }

    public Keywords(string data) {
        hasBreakthrough = data.charAt(0) == 'B';
        hasCharge = data.charAt(1) == 'C';
        hasDrain = data.charAt(2) == 'D';
        hasGuard = data.charAt(3) == 'G';
        hasLethal = data.charAt(4) == 'L';
        //hasRegenerate = data.charAt(5) == 'R';
        hasWard = data.charAt(5) == 'W';
    }

    public Keywords(Keywords keywords) {
        hasBreakthrough = keywords.hasBreakthrough;
        hasCharge = keywords.hasCharge;
        hasDrain = keywords.hasDrain;
        hasGuard = keywords.hasGuard;
        hasLethal = keywords.hasLethal;
        //hasRegenerate = keywords.hasRegenerate;
        hasWard = keywords.hasWard;
    }

    public string ToString() {
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
