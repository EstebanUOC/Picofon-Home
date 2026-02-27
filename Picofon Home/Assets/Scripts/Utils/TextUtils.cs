using System;

public static class TextUtils
{
    public static string RemoveAccentsAndPrepend(string input, string prefix)
    {
        int totalLength = prefix.Length + input.Length;

        return string.Create(
            totalLength,
            (prefix, input),
            (span, state) =>
            {
                var (pref, src) = state;

                pref.AsSpan().CopyTo(span);

                int w = pref.Length;

                foreach (char c in src)
                {
                    span[w++] = RemoveAccent(c);
                }
            }
        );
    }

    public static string RemoveAccentsAndAppend(string input, string suffix)
    {
        int totalLength = input.Length + suffix.Length;

        return string.Create(
            totalLength,
            (input, suffix),
            (span, state) =>
            {
                var (src, suf) = state;

                int w = 0;

                // Procesar input quitando acentos
                foreach (char c in src)
                {
                    span[w++] = RemoveAccent(c);
                }

                // Copiar suffix directo
                suf.AsSpan().CopyTo(span[w..]);
            }
        );
    }

    private static char RemoveAccent(char c)
    {
        return c switch
        {
            // a
            'á' or 'à' or 'ä' or 'â' or 'ã' => 'a',
            'Á' or 'À' or 'Ä' or 'Â' or 'Ã' => 'A',

            // e
            'é' or 'è' or 'ë' or 'ê' => 'e',
            'É' or 'È' or 'Ë' or 'Ê' => 'E',

            // i
            'í' or 'ì' or 'ï' or 'î' => 'i',
            'Í' or 'Ì' or 'Ï' or 'Î' => 'I',

            // o
            'ó' or 'ò' or 'ö' or 'ô' or 'õ' => 'o',
            'Ó' or 'Ò' or 'Ö' or 'Ô' or 'Õ' => 'O',

            // u
            'ú' or 'ù' or 'ü' or 'û' => 'u',
            'Ú' or 'Ù' or 'Ü' or 'Û' => 'U',

            'ñ' => 'n',
            'Ñ' => 'N',
            'ç' => 'c',
            'Ç' => 'C',

            _ => c,
        };
    }
}
