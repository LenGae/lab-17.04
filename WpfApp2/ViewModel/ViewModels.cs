using System.Collections.ObjectModel;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModel;

public class ViewModels : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly PhoneBookContext _context;

    public ObservableCollection<Contact> Contacts { get; set; }

    private string _name = "";

    public string Name
    {
        get => _name;
        set
        {
            Set(ref _name, value);
            AddCommand.NotifyCanExecuteChanged();
            UpdateCommand.NotifyCanExecuteChanged();
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
            UpdateCommand.NotifyCanExecuteChanged();
        }
    }

    private Contact? _selectedContact;

    public Contact? SelectedContact
    {
        get => _selectedContact;
        set
        {
            Set(ref _selectedContact, value);

            if (value != null)
            {
                Name = value.Name;
                Phone = value.Phone;
            }

            DeleteCommand.NotifyCanExecuteChanged();
            UpdateCommand.NotifyCanExecuteChanged();
        }
    }

    public RelayCommand AddCommand { get; }

    public RelayCommand DeleteCommand { get; }

    public RelayCommand UpdateCommand { get; }

    public ViewModels(IDialogService dialogService,
        PhoneBookContext context)
    {
        _dialogService = dialogService;
        _context = context;

        try
        {
            var databaseContacts =
                _context.Contacts.ToList();

            Contacts =
                new ObservableCollection<Contact>(
                    databaseContacts);
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(
                "Ошибка подключения к БД: " + ex.Message);

            Contacts = new ObservableCollection<Contact>();
        }

        AddCommand =
            new RelayCommand(Add, CanAdd);

        DeleteCommand =
            new RelayCommand(Delete, CanDelete);

        UpdateCommand =
            new RelayCommand(Update, CanUpdate);
    }

    private void Add()
    {
        try
        {
            if (_context.Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning(
                    "Контакт с таким номером уже существует!");

                return;
            }

            var newContact = new Contact
            {
                Name = Name,
                Phone = Phone
            };

            _context.Contacts.Add(newContact);

            _context.SaveChanges();

            Contacts.Add(newContact);

            _dialogService.ShowInfo(
                "Контакт успешно добавлен!");

            Name = "";
            Phone = "";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(
                "Ошибка при добавлении: " + ex.Message);
        }
    }

    private bool CanAdd()
    {
        return !string.IsNullOrWhiteSpace(Name)
            && !string.IsNullOrWhiteSpace(Phone);
    }

    private void Update()
    {
        if (SelectedContact == null)
            return;

        try
        {
            SelectedContact.Name = Name;
            SelectedContact.Phone = Phone;

            _context.SaveChanges();

            _dialogService.ShowInfo(
                "Контакт успешно обновлён!");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(
                "Ошибка при обновлении: " + ex.Message);
        }
    }

    private bool CanUpdate()
    {
        return SelectedContact != null;
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

        try
        {
            _context.Contacts.Remove(SelectedContact);

            _context.SaveChanges();

            Contacts.Remove(SelectedContact);

            _dialogService.ShowInfo(
                "Контакт удалён.");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(
                "Ошибка при удалении: " + ex.Message);
        }
    }

    private bool CanDelete()
    {
        return SelectedContact != null;
    }
}