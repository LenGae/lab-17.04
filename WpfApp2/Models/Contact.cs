using System.Text.RegularExpressions;

namespace WpfApp2.Models;

public class Contact : ObservableObject
{
    private string _name = "";
    private string _phone = "";

    public Contact(string name, string phone)
    {
        _name = name;
        _phone = phone;

        if (!Validate())
            throw new ArgumentException("Некорректное имя или телефон");
    }

    public string Name
    {
        get => _name;
        set
        {
            Set(ref _name, value);
            if (!Validate())
                throw new ArgumentException("Некорректное имя");
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            Set(ref _phone, value);
            if (!Validate())
                throw new ArgumentException("Некорректный телефон");
        }
    }

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return false;

        return Regex.IsMatch(Phone, @"^(\+7\d{10}|\d{10,11})$");
    }
}