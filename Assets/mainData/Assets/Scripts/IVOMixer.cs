public interface IVOMixer
{
	void Update();

	void SetOutput(IVOMixer output);

	void SendVO(IVOMixerItem item);
}
