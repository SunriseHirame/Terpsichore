namespace Hirame.Terpsichore
{
    public interface ITween
    {
        void Play ();
        
        void Stop ();
        
        void Pause ();
        
        void SetTime (float t);
    }

}