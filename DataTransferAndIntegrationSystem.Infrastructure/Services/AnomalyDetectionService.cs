using System.Text.RegularExpressions;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;

namespace DataTransferAndIntegrationSystem.Infrastructure.Services;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    public AnomalyResultDto ValidateUser(ExternalUserDto user)
    {
        var result = new AnomalyResultDto();

        ValidateName(
            user.FirstName,
            user.LastName,
            result);

        ValidateEmail(
            user.Email,
            result);

        ValidatePhone(
            user.Phone,
            result);

        return result;
    }

    private void ValidateName(
    string firstName,
    string lastName,
    AnomalyResultDto result)
    {
        var fullName = $"{firstName} {lastName}".Trim();

        if (fullName.Length < 2)
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Name",
        Message = "Name is too short."
    });

            return;
        }

        if (Regex.IsMatch(fullName, @"\d"))
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Name",
        Message = "Name contains numbers."
    });

            return;
        }

        string[] suspiciousNames =
        {
            "test",
            "admin",
            "user",
            "unknown",
            "asdf",
            "qwerty",
            "asdasd"
        };

        if (suspiciousNames.Any(x =>
            fullName.ToLower().Contains(x)))
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Name",
        Message = "Suspicious name detected."
    });

            return;
        }

        if (Regex.IsMatch(fullName, @"(.)\1{4,}"))
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Name",
        Message = "Name contains repeated characters."
    });

            return;
        }


        if (Regex.IsMatch(fullName, @"^(.+)\1+$"))
        {
            result.Errors.Add(
                new AnomalyErrorDto
                {
                    Field = "Name",
                    Message = "Name contains repeated pattern."
                });

            return;
        }




    }

    private void ValidateEmail(string email, AnomalyResultDto result)
    {

        string[] suspiciousEmails =
        {
            "test",
            "admin",
            "example",
            "fake"
        };

        if (suspiciousEmails.Any(x =>
            email.ToLower().Contains(x)))
        {
            result.Errors.Add(
   new AnomalyErrorDto
   {
       Field = "Email",
       Message = "Suspicious email detected."
   });

            return;
        }


    }

    private void ValidatePhone(string phone, AnomalyResultDto result)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Phone",
        Message = "Phone number is empty."
    });

            return;
        }

        string digits =
            new string(phone.Where(char.IsDigit).ToArray());

        if (digits.Length < 10)
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Phone",
        Message = "Phone number is too short."
    });

            return;
        }

        if (digits.Distinct().Count() == 1)
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Phone",
        Message = "Phone number contains repeated digits."
    });

            return;
        }

        if (digits == "1234567890")
        {
            result.Errors.Add(
    new AnomalyErrorDto
    {
        Field = "Phone",
        Message = "Sequential phone number detected."
    });

            return;
        }


    }


}