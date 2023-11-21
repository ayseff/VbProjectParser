using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Data.Base;

public class ValidationResult
{
    public bool IsValid { get; protected set; }
    public IEnumerable<Exception> Exceptions => _Exceptions;

    protected List<Exception> _Exceptions = new();

    /// <summary>
    /// Successful validation
    /// </summary>
    public ValidationResult()
    {
        IsValid = true;
    }

    /// <summary>
    /// Unsuccessful validation
    /// </summary>
    public ValidationResult(Exception ex)
    {
        IsValid = false;
        _Exceptions.Add(ex);
    }

    /// <summary>
    /// Unsuccessful validation
    /// </summary>
    public ValidationResult(IEnumerable<Exception> exceptions)
    {
        IsValid = false;
        _Exceptions.AddRange(exceptions);
    }

    public void Merge(ValidationResult Other)
    {
        if (!Other.IsValid)
            IsValid = false;

        _Exceptions.AddRange(Other.Exceptions);
    }
}
