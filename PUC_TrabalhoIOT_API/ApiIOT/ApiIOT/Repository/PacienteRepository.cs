using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Net;
using ApiIOT.Models;
namespace ApiIOT.Repository
{
    public class PacienteRepository
    {
        // ADD THIS PART TO YOUR CODE
        private string EndpointUri = "<your endpoint URI>";
        private string PrimaryKey = "<your key>";
        private DocumentClient client;

        public PacienteRepository() {
            EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];
            PrimaryKey  = ConfigurationManager.AppSettings["PrimaryKey"];

            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
        }

        private async Task CreateDatabaseIfNotExists(string databaseName)
        {
            // Check to verify a database with the id=FamilyDB does not exist
            try
            {
                ResourceResponse<Database> r = await this.client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));


            }
            catch (DocumentClientException de)
            {
                string teste = de.StatusCode.ToString();

                if (de.StatusCode == HttpStatusCode.NotFound)
                    this.client.CreateDatabaseAsync(new Database { Id = databaseName });
            }
        }

        private async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                ResourceResponse<DocumentCollection> r = await this.client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                string teste = de.StatusCode.ToString();

                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    // Configure collections for maximum query flexibility including string range queries.
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    // Here we create a collection with 400 RU/s.
                    this.client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        collectionInfo,
                        new RequestOptions { OfferThroughput = 400 });
                }
            }
        }

        private const string DATABASE   = "MedCare";
        private const string COLLECTION = "PacienteCollection";

        private async Task Base()
        {
            await this.CreateDatabaseIfNotExists(DATABASE);
            await this.CreateDocumentCollectionIfNotExists(DATABASE, COLLECTION);
        }

        public async Task<string> CadastrarAsync(Paciente paciente)
        {
            try
            {
                await Base();

                if (string.IsNullOrWhiteSpace(paciente.Id))
                    paciente.Id = Guid.NewGuid().ToString();

                ResourceResponse<Document> r = await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DATABASE, COLLECTION, paciente.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DATABASE, COLLECTION), paciente);
                }

                string teste = de.StatusCode.ToString();
            }

            return paciente.Id;
        }

        public async Task AlterarAsync(string Id, Paciente paciente)
        {
            try
            {
                await Base();
                await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DATABASE, COLLECTION, Id), paciente);
            }
            catch (DocumentClientException)
            {
                throw;
            }
        }

        public async Task<Paciente[]> BuscarPacienteAsync()
        {
            await Base();

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Here we find the Andersen family via its LastName
            IQueryable<Paciente> pacienteQuery = this.client.CreateDocumentQuery<Paciente>(
                    UriFactory.CreateDocumentCollectionUri(DATABASE, COLLECTION), queryOptions);

            return pacienteQuery.ToArray();
        }

        public async Task<Paciente> BuscarPacienteAsync(string Id)
        {
            await Base();

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
     
            // Here we find the Andersen family via its LastName
            IQueryable<Paciente> pacienteQuery = this.client.CreateDocumentQuery<Paciente>(
                    UriFactory.CreateDocumentCollectionUri(DATABASE, COLLECTION), queryOptions)
                    .Where(p => p.Id.Equals(Id));

            IEnumerable<Paciente> resultado = pacienteQuery.ToList();

            return resultado.FirstOrDefault();
        }
    }
}