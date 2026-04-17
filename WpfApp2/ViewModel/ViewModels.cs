using System.Collections.ObjectModel;
using WpfApp2.Models;

namespace WpfApp2.ViewModel;

public class ViewModels : ObservableObject
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            Set(ref _name, value);
            AddCommand.NotifyCanExecuteChanged();
        }
    }

    private string _phone = "";
    public string Phone
    {
        get => _phone;
        set
        {
            Set(ref _phone, value);
            AddCommand.NotifyCanExecuteChanged();
        }
    }

    public ObservableCollection<Contact> Contacts { get; } = [];

    private Contact? _selectedContact;
    public Contact? SelectedContact
    {
        get => _selectedContact;
        set
        {
            Set(ref _selectedContact, value);
            DeleteCommand.NotifyCanExecuteChanged();
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public ViewModels()
    {
        AddCommand = new RelayCommand(Add, CanAdd);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
    }

    private void Add()
    {
        try
        {
            Contacts.Add(new Contact(Name, Phone));
            Name = "";
            Phone = "";
        }
        catch { }
    }

    private bool CanAdd()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Phone);
    }

    private void Delete()
    {
        if (SelectedContact != null)
            Contacts.Remove(SelectedContact);
    }

    private bool CanDelete()
    {
        return SelectedContact != null;
    }
}