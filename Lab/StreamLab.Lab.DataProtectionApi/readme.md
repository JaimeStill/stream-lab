# Data Protection API

**Initial Proof of Concept**  

https://github.com/JaimeStill/stream-lab/assets/14102723/148ebbd7-1aac-4776-9e3e-9156d7b0ec14

The above demonstration illustrates:

* Creating a [`Mark`](./Schema/Mark.cs) record.
* Creating a [`MarkedString`](./Schema/MarkedString.cs) record encrypted with the associated marking: *Secret*.
* Decrypting the payload of the created `MarkedString` revealing the stored launch codes.
* Refreshing the table data in the database showing that only encrypted values are stored (`Mark` table is on the top, `MarkedString` table is on the bottom).

## Concept

[`Mark`](./Schema/Mark.cs) - Stores an [AES](https://learn.microsoft.com/en-us/dotnet/standard/security/cryptographic-services#secret-key-encryption) encryption key and initialization vector as encrypted base 64 strings and is used to associate data to a given marking (i.e. - *Confidential*, *Secret*, etc.). The encryption key and initialization vector are encrypted using `Mark.Value` against the [ASP.NET Core Data Protection API](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-7.0). This facilitates the establishment of a [Hybrid Cryptosystem](https://en.wikipedia.org/wiki/Hybrid_cryptosystem).

* [`MarkService`](./Services/MarkService.cs)
* [`MarkController`](./Controllers/MarkController.cs)

[`MarkedData`](./Schema/MarkedData.cs) - A base class that is intended to associate encrypted data in various formats with the `Mark` that was used to encrypt its payload. Data is encrypted and stored as base 64 string in the `Payload` property.

* [`MarkedDataService`](./Services/MarkedDataService.cs)
* [`MarkedDataController`](./Controllers/MarkedDataController.cs)

[`MarkedString`](./Schema/MarkedString.cs) - Derives from `MarkedData` and defines the ability to store encrypted strings in any format (simple text, JSON, etc.).

* [`MarkedStringService`](./Services/MarkedStringService.cs)
* [`MarkedStringController`](./Controllers/MarkedStringController.cs)

### Future Data Formats

* [`MarkedObject`](./Schema/MarkedObject.cs) - Will define the ability to store a serialized C# object that can be deserialized into an object once decrypted.
* [`MarkedFile`](./Schema/MarkedFile.cs) - Will define the ability to store binary file data that can be rehydrated into its native format once decrypted.