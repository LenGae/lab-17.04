using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModel;

public class ViewModels : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly IDbContextFactory<PhoneBookContext> _contextFactory;

    public ObservableCollection<Contact> Contacts { get; set; }
        = new ObservableCollection<Contact>();

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

    public ViewModels(
        IDialogService dialogService,
        IDbContextFactory<PhoneBookContext> contextFactory)
    {
        _dialogService = dialogService;
        _contextFactory = contextFactory;

        LoadContacts();

        AddCommand = new RelayCommand(Add, CanAdd);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
        UpdateCommand = new RelayCommand(Update, CanUpdate);
    }

    private void LoadContacts()
    {
        try
        {
            using var context =
                _contextFactory.CreateDbContext();

            Contacts = new ObservableCollection<Contact>(
                context.Contacts.ToList());
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(
                "Ошибка подключения к БД: " + ex.Message);

            Contacts = new ObservableCollection<Contact>();
        }
    }

    private void Add()
    {
        try
        {
            using var context =
                _contextFactory.CreateDbContext();

            if (context.Contacts.Any(c => c.Phone == Phone))
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

            context.Contacts.Add(newContact);
            context.SaveChanges();

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
            using var context =
                _contextFactory.CreateDbContext();

            var contactToUpdate =
                context.Contacts.Find(SelectedContact.Id);

            if (contactToUpdate == null)
            {
                _dialogService.ShowWarning(
                    "Контакт не найден!");

                return;
            }

            contactToUpdate.Name = Name;
            contactToUpdate.Phone = Phone;

            context.SaveChanges();

            SelectedContact.Name = Name;
            SelectedContact.Phone = Phone;

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
            using var context =
                _contextFactory.CreateDbContext();

            var contactToDelete =
                context.Contacts.Find(SelectedContact.Id);

            if (contactToDelete == null)
            {
                _dialogService.ShowWarning(
                    "Контакт не найден!");

                return;
            }

            context.Contacts.Remove(contactToDelete);
            context.SaveChanges();

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