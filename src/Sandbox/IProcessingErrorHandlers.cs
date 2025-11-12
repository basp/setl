namespace Sandbox;

internal interface IProcessingErrorHandlers
{
    void OnOnbekendeParser(Line line);

    void OnMislukteParse(Line line);
    
    void OnOnbekendeValidatie(Line line);

    void OnOnbekendeEvaluatie(Line line);

    void OnMislukteEvaluatie(Line line);
    
    void OnOnbekendeConversie(Line line);
    
    void OnMislukteConversie(Line line, Exception ex);
}