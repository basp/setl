using Sandbox.Records;

namespace Sandbox;

internal interface IRecordVisitor
{
    void VisitBericht(Berichtrecord record);
    
    void VisitGemeente(Gemeenterecord record);
    
    void VisitDetail(Detailrecord record);
    
    void VisitTellingen(Tellingenrecord record);
}