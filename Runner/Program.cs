using DataLayer;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Runner
{
    internal static class Program
    {
        private static IConfigurationRoot config;

        private static void Main(string[] args)
        {
            //se inicializa para registrar el appSetting
            Initialize();

            //Get_all_should_return_6_results();

            Find_should_retrieve_existing_entity(1);

            //var id = Insert_should_assign_identity_to_new_entity();
            //Find_should_retrieve_existing_entity(id);
            //Modify_should_update_existing_entity(id);
            //Delete_should_remove_entity(id);

            //var repository = CreateRepository();
            //var mj = repository.GetFullContact(1);
            //mj.Output();
        }

        private static void Delete_should_remove_entity(int id)
        {
            // obtener conexion
            IContactRepository repository = CreateRepository();

            // eliminar por id
            repository.Remove(id);

            // crear un nuevo repositorio para propósitos de verificación
            IContactRepository repository2 = CreateRepository();
            var deletedEntity = repository2.Find(id);

            // assert
            Debug.Assert(deletedEntity == null);
            Console.WriteLine("*** Contact Deleted ***");
        }

        //modificar o actualizar
        private static void Modify_should_update_existing_entity(int id)
        {
            IContactRepository repository = CreateRepository();

            var contact = repository.GetFullContact(id);
            contact.FirstName = "Bob";
            contact.Addresses[0].StreetAddress = "456 Main Street";

            //Acutalizar o Guardar

            //repository.Update(contact);
            repository.Save(contact);

            // verificacion
            IContactRepository repository2 = CreateRepository();
            //var modifiedContact = repository2.Find(id);
            var modifiedContact = repository2.GetFullContact(id);

            Console.WriteLine("*** Contact Modified ***");
            modifiedContact.Output();
            Debug.Assert(modifiedContact.FirstName == "Bob");
            Debug.Assert(modifiedContact.Addresses.First().StreetAddress == "456 Main Street");
        }

        // obtener todo el contacto
        private static void Find_should_retrieve_existing_entity(int id)
        {
            //crear obtener conexion
            IContactRepository repository = CreateRepository();

            // act
            //var contact = repository.Find(id);
            var contact = repository.GetFullContact(id);

            // assert
            Console.WriteLine("*** Get Contact ***");
            contact.Output();

            //Test

            //Debug.Assert(contact.FirstName == "Joe");
            //Debug.Assert(contact.LastName == "Blow");
            //Debug.Assert(contact.Addresses.Count == 1);
            //Debug.Assert(contact.Addresses.First().StreetAddress == "123 Main Street");
        }

        private static int Insert_should_assign_identity_to_new_entity()
        {
            
            IContactRepository repository = CreateRepository();
            var contact = new Contact
            {
                FirstName = "Joe",
                LastName = "Blow",
                Email = "joe.blow@gmail.com",
                Company = "Microsoft",
                Title = "Developer"
            };
            var address = new Address
            {
                AddressType = "Home",
                StreetAddress = "123 Main Street",
                City = "Baltimore",
                StateId = 1,
                PostalCode = "22222"
            };
            contact.Addresses.Add(address);

            // act
            //repository.Add(contact);
            repository.Save(contact);

            // assert
            Debug.Assert(contact.Id != 0);
            Console.WriteLine("*** Contact Inserted ***");
            Console.WriteLine($"New ID: {contact.Id}");
            return contact.Id;
        }

        private static void Get_all_should_return_6_results()
        {
            //se ob
            var repository = CreateRepository();

            var contacts = repository.GetAll();

            Console.WriteLine($"Count: {contacts.Count}");
            Debug.Assert(contacts.Count == 6);
            contacts.Output();
        }

        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config = builder.Build();
        }

        private static IContactRepository CreateRepository()
        {
            return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            //return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));
        }
    }
}