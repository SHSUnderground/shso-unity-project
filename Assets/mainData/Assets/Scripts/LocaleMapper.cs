using System.Collections.Generic;

public class LocaleMapper
{
	protected static string[] availableLocales = new string[10]
	{
		"en-us",
		"de",
		"es",
		"es-co",
		"fr",
		"pt",
		"pt-br",
		"ru",
		"tr",
		"it"
	};

	public static string[] AvailableLocales
	{
		get
		{
			return availableLocales;
		}
	}

	public static string GetCurrentLocale()
	{
		string text = AppShell.Instance.Locale;
		if (text == null || text == string.Empty)
		{
			text = "en";
		}
		return CultureInfoToLocale(text);
	}

	public static string GetCurrentLocaleDirectory()
	{
		string currentLocale = GetCurrentLocale();
		return (!(currentLocale == "en-us")) ? currentLocale : "en_us";
	}

	public static string CultureInfoToLocale(string inName)
	{
		string text = inName.ToLower();
		string[] array = text.Split('-');
		string[] array2 = AvailableLocales;
		foreach (string text2 in array2)
		{
			if (text == text2)
			{
				return text2;
			}
		}
		string[] array3 = AvailableLocales;
		foreach (string text3 in array3)
		{
			string[] array4 = text3.Split('-');
			if (array[0] == array4[0])
			{
				return text3;
			}
		}
		return "en-us";
	}

	public static string[] LocaleToDirectories(string localeName)
	{
		return LocaleToDirectories(localeName, true);
	}

	public static string[] LocaleToDirectories(string localeName, bool roots)
	{
		Stack<string> stack = new Stack<string>();
		if (roots)
		{
			stack.Push(string.Format("{0}/", "GUI/i18n"));
			stack.Push(string.Format("{0}/{1}/", "GUI/i18n", "non_locale"));
		}
		string arg = CultureInfoToLocale(localeName);
		string text = string.Format("{0}/{1}/", "GUI/i18n", arg);
		text = text.Replace("en-us", "en_us");
		stack.Push(text);
		return stack.ToArray();
	}
}
