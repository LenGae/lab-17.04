using System.Collections.ObjectModel;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModel;

public class ViewModels : ObservableObject
{
    private readonly IDialogService _dialogService;

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

    public ViewModels(IDialogService dialogService)
    {
        _dialogService = dialogService;

        AddCommand = new RelayCommand(Add, CanAdd);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
    }

    private void Add()
    {
        try
        {
            if (Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning(
                    "Контакт с таким номером уже существует!");

                return;
            }

            Contacts.Add(new Contact(Name, Phone));

            _dialogService.ShowInfo(
                "Контакт успешно добавлен!");

            Name = "";
            Phone = "";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(ex.Message);
        }
    }

    private bool CanAdd()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Phone);
    }

    private void Delete()
    {
        if (SelectedContact == null)
            return;

        bool confirm =
            _dialogService.ShowConfirmation(
                $"Удалить контакт {SelectedContact.Name}?");

        if (!confirm)
            return;

        Contacts.Remove(SelectedContact);

        _dialogService.ShowInfo(
            "Контакт удалён.");
    }

    private bool CanDelete()
    {
        return SelectedContact != null;
    }
}