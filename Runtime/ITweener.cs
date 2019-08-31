namespace Hirame.Terpsichore
{
    public interface ITweener
    {
        void Play ();
        
        void Stop ();
        
        void Pause ();

        void OnUpdateTween ();
        
        void SetTime (float t);
        
        bool IsRunning { get; }
    }

}