using System.Collections.ObjectModel;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModel;

public class ViewModels : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly PhoneBookContext _context;

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

    public ViewModels(IDialogService dialogService, PhoneBookContext context)
    {
        _dialogService = dialogService;
        _context = context;

        try
        {
            var databaseContacts = _context.Contacts.ToList();
            Contacts = new ObservableCollection<Contact>(databaseContacts);
        }
        catch (Exception ex)
        {
            _dialogService.ShowError("Ошибка подключения к БД: " + ex.Message);
            Contacts = new ObservableCollection<Contact>();
        }

        AddCommand = new RelayCommand(Add, CanAdd);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
    }

    private void Add()
    {
        try
        {
            if (_context.Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            var newContact = new Contact { Name = this.Name, Phone = this.Phone };

            _context.Contacts.Add(newContact);
            _context.SaveChanges();

            Contacts.Add(newContact);

            _dialogService.ShowInfo("Контакт успешно добавлен в базу!");

            Name = "";
            Phone = "";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError("Ошибка при сохранении: " + ex.Message);
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