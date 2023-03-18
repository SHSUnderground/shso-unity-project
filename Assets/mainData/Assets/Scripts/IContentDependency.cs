using System.Collections.Generic;

internal interface IContentDependency
{
	List<ContentReference> ContentReferenceList
	{
		get;
	}

	bool IsContentLoaded
	{
		get;
	}

	bool IsContentDependent
	{
		get;
	}

	ContentLoadingActivateDelegate OnLoadingActivate
	{
		get;
		set;
	}

	void ConfigureRequiredContent(List<ContentReference> ContentReferenceList);

	void ConfigureRequiredContent(ContentReference ContentReferenceList);

	void ConfigureRequiredContent();
}
