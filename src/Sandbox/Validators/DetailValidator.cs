namespace Sandbox.Validators;

internal class DetailValidator : DataValidator
{
    // Onbeschikbare datum velden kunnen uit acht (8) nullen (0) bestaan.
    private static readonly string ZeroDate = string.Empty.PadLeft(8, '0');
    
    private readonly IValidationErrorHandlers handlers;

    public DetailValidator(IValidationErrorHandlers handlers)
    {
        this.handlers = handlers;   
    }

    public override bool Validate(Line line, Dictionary<string,string> data)
    {
        var valid = true;

        valid = valid && this.ValidateHoofdpersoon(line, data);
        
        var hasPartner = !string.IsNullOrWhiteSpace(data[KnownFields.BsnPartner]);
        if (hasPartner)
        {
            valid = valid && this.ValidatePartner(line, data);
        }
        
        if (!this.ValidateDate(data[KnownFields.IngangsdatumRecht], "yyyyMMdd"))
        {
            valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnIngangsdatumRechtNietGeldig(context);
        }
		
        if (this.IsDatumAanwezig(data[KnownFields.EinddatumRecht]))
        {
            if (!this.ValidateDate(
                data[KnownFields.EinddatumRecht], 
                "yyyyMMdd"))
            {
                valid = false;
                var context = new ValidationErrorContext(line, data);
                this.handlers.OnEinddatumRechtNietGeldig(context);
            }
        }
        
        return valid;
    }

	private bool IsDatumAanwezig(string value) =>
		!string.IsNullOrWhiteSpace(value) && !IsZeroDate(value);

	private bool ValidateHoofdpersoon(
        Line line, 
        Dictionary<string, string> data)
	{
		var valid = true;
		
		if (!this.ValidateNumeric(data[KnownFields.BsnHoofdpersoon]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
			this.handlers.OnBsnHpNietNumeriek(context);
		}
		else if (!this.Validate11Proef(data[KnownFields.BsnHoofdpersoon]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnBsnHpNiet11Proef(context);
		}

		if (!this.ValidateDate(
            data[KnownFields.GeboortedatumHoofdpersoon], 
            "yyyyMMdd"))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnGeboortedatumHpNietGeldig(context);
		}

		if (!this.ValidateNumeric(data[KnownFields.WwbBedragHoofdpersoon]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnWwbBedragHpNietNumeriek(context);
		}

		if (!this.ValidateNumeric(data[KnownFields.PostcodeNumeriek]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnPostcodeNumeriekNietNumeriek(context);
		}

		if (!this.ValidateNumeric(data[KnownFields.Huisnummer]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnHuisnummerNietNumeriek(context);
		}
		
		return valid;
	}
	
	private bool ValidatePartner(
        Line line, 
        Dictionary<string, string> data)
	{
		var valid = true;
		
		if (!this.ValidateNumeric(data[KnownFields.BsnPartner]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnBsnPNietNumeriek(context);
		}
		else if (!this.Validate11Proef(data[KnownFields.BsnPartner]))
		{
			valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnBsnPNiet11Proef(context);
		}
		
		if (this.IsDatumAanwezig(data[KnownFields.GeboortedatumPartner]))
		{
			if (!this.ValidateDate(
                data[KnownFields.GeboortedatumPartner], 
                "yyyyMMdd"))
			{
				valid = false;
                var context = new ValidationErrorContext(line, data);
                this.handlers.OnGeboortedatumPNietGeldig(context);
			}			
		}
		
		if (!string.IsNullOrWhiteSpace(data[KnownFields.WwbBedragPartner]))
		{
			if (!this.ValidateNumeric(data[KnownFields.WwbBedragPartner]))
			{
				valid = false;
                var context = new ValidationErrorContext(line, data);
                this.handlers.OnWwbBedragPNietNumeriek(context);
			}
		}
		
		return valid;
	}

	private static bool IsZeroDate(string value) => value == ZeroDate;
}