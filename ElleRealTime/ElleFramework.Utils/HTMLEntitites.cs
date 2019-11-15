using System.Collections.Generic;

namespace ElleFramework.Utils
{
    public class HTMLEntities
    {
        private readonly Dictionary<string, string> entities = new Dictionary<string, string>(256);

        public HTMLEntities()
        {
            AddEntity("\"", "&quot;");
            AddEntity("&", "&amp;");
            AddEntity("<", "&lt;");
            AddEntity(">", "&gt;");
            AddEntity(" ", "&nbsp;"); // non è uno spazio quello che non si vede!
            AddEntity("¡", "&iexcl;");
            AddEntity("¢", "&cent;");
            AddEntity("£", "&pound;");
            AddEntity("¤", "&curren;");
            AddEntity("¥", "&yen;");
            AddEntity("¦", "&brvbar;");
            AddEntity("§", "&sect;");
            AddEntity("¨", "&uml;");
            AddEntity("©", "&copy;");
            AddEntity("ª", "&ordf;");
            AddEntity("«", "&laquo;");
            AddEntity("¬", "&not;");
            AddEntity("­", "&shy;");
            AddEntity("®", "&reg;");
            AddEntity("¯", "&macr;");
            AddEntity("°", "&deg;");
            AddEntity("±", "&plusmn;");
            AddEntity("²", "&sup2;");
            AddEntity("³", "&sup3;");
            AddEntity("´", "&acute;");
            AddEntity("µ", "&micro;");
            AddEntity("¶", "&para;");
            AddEntity("·", "&middot;");
            AddEntity("¸", "&cedil;");
            AddEntity("¹", "&sup1;");
            AddEntity("º", "&ordm;");
            AddEntity("»", "&raquo;");
            AddEntity("¼", "&frac14;");
            AddEntity("½", "&frac12;");
            AddEntity("¾", "&frac34;");
            AddEntity("¿", "&iquest;");
            AddEntity("¾", "&frac34;");
            AddEntity("¿", "&iquest;");
            AddEntity("À", "&Agrave;");
            AddEntity("Á", "&Aacute;");
            AddEntity("Â", "&Acirc;");
            AddEntity("Ã", "&Atilde;");
            AddEntity("Ä", "&Auml;");
            AddEntity("Å", "&Aring;");
            AddEntity("Æ", "&AElig;");
            AddEntity("Ç", "&Ccedil;");
            AddEntity("È", "&Egrave;");
            AddEntity("É", "&Eacute;");
            AddEntity("Ê", "&Ecirc;");
            AddEntity("Ë", "&Euml;");
            AddEntity("Ì", "&Igrave;");
            AddEntity("Í", "&Iacute;");
            AddEntity("Î", "&Icirc;");
            AddEntity("Ï", "&Iuml;");
            AddEntity("Ð", "&ETH;");
            AddEntity("Ñ", "&Ntilde;");
            AddEntity("Ò", "&Ograve;");
            AddEntity("Ó", "&Oacute;");
            AddEntity("Ô", "&Ocirc;");
            AddEntity("Õ", "&Otilde;");
            AddEntity("Ö", "&Ouml;");
            AddEntity("×", "&times;");
            AddEntity("Ø", "&Oslash;");
            AddEntity("Ù", "&Ugrave;");
            AddEntity("Ú", "&Uacute;");
            AddEntity("Û", "&Ucirc;");
            AddEntity("Ü", "&Uuml;");
            AddEntity("Ý", "&Yacute;");
            AddEntity("Þ", "&THORN;");
            AddEntity("ß", "&szlig;");
            AddEntity("à", "&agrave;");
            AddEntity("á", "&aacute;");
            AddEntity("â", "&acirc;");
            AddEntity("ã", "&atilde;");
            AddEntity("ä", "&auml;");
            AddEntity("å", "&aring;");
            AddEntity("æ", "&aelig;");
            AddEntity("ç", "&ccedil;");
            AddEntity("è", "&egrave;");
            AddEntity("é", "&eacute;");
            AddEntity("ê", "&ecirc;");
            AddEntity("ë", "&euml;");
            AddEntity("ì", "&igrave;");
            AddEntity("í", "&iacute;");
            AddEntity("î", "&icirc;");
            AddEntity("ï", "&iuml;");
        }

        private void AddEntity(string ch, string entity)
        {
            entities[ch] = entity;
        }

        public string GetEntity(string ch)
        {
            string ret = entities[ch];

            if (ret == null)
                ret = ch;

            return ret;
        }
    }
}
