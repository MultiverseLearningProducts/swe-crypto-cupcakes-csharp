# SWE Crypto Cupcakes - C#

**See setup instructions on the `main` branch to install dependencies and start
the ASP.NET Core server.**

This branch deals with two security measures:

- encrypting the cupcake instructions
- creating user accounts

We have also refactored into separate routes for readability.

## Coach notes

The big concepts at play are

- [encryption](https://mv-swe-docs.netlify.app/backend/encryption)
- [hashing](https://mv-swe-docs.netlify.app/backend/hashing)
- the [basic auth](https://mv-swe-docs.netlify.app/backend/basic-auth.html)
  protocol

The primers linked to above are designed for colleagues to brush up on the
details, but it's fine to share them with apprentices to if you think they would
appreciate any of the details.

## Things to see and do

### EncryptUtility.cs

#### Encrypting cupcake instructions

In `Utilities/EncryptUtility.cs` we can see the helper functions which encrypt
and decrypt data. There's quite a lot going on here and apprentices might want
to search for something simpler but less secure.

In order to use the functions, you will need to generate a 16-byte key (32 hexadecimal characters). Recall that **1 byte = 8 bits**, so 16 bytes is 128 bits, as required by the SHA256 algorithm that AES is based on. The function expects these 16 bytes as hex. You will also need to generate a 8-byte key (16 hexadecimal characters) for the initialization vector, using a similar process.

Generate the AES Key:

```bash
openssl rand -hex 16
```

This would give us

```bash
22199a2ce17b2bcbbe4e280b0af97218
```

(You will notice that these two two times the number of characters (e.g. 32) than bytes (e.g. 16). This is because 1 byte in hex is represented by a pair of characters.)

Generate the AES IV:

```bash
openssl rand -hex 8
```

This would give us

```bash
f38f89759ae6da84
```

Save your key in the `secrets.json` file, then you can add some code to `EncryptUtility.cs`.

It is worth talking about why we put this in a `secrets.json` file and let them see you doing this step. Additionally note to apprentices that these secrets are normally not committed to the repository because of the sensitivity of the values such as the encryption key. For demonstration purposes this is being committed, rather than each coach having to work with different versions of encrypted data with different encryption keys.

### CupcakeController.cs

In `CupcakeController.cs` we can see that new instructions are now being encrypted!

In `seedData.json` we can also see that the instructions in the seed data are encrypted as well!

Try adding `Console.Write` in the `POST /cupcakes` endpoint so you can see the data
which actually gets stored, then try adding a new cupcake using the bash command from the previous week's lesson:

```bash
curl -v -XPOST \
-H "Content-type: application/json" \
-d '{ "flavor" : "marble", "instructions" : "freeze for 24 hours beforehand" }' \
'http://localhost:7119/cupcakes' | json_pp
```

Notice that the data is encrypted in the data store. If you try retrieving the same cupcake you added: the instructions will be decrypted before being returned by the API

### UserController.cs

#### Creating a user

To create a user, hit

```bash
curl -v -XPOST \
-H 'Authorization: Basic dGVzdEB1c2VyLmNvbTpwYXNzd29yZDEyMw==' \
'http://localhost:7119/users' | json_pp
```

Note that `dGVzdEB1c2VyLmNvbTpwYXNzd29yZDEyMw==` is the Base 64 encoding of the
string `'test@user.com:password123'`. This is the standard way of sending
credentials with basic auth. See
[basic auth](https://mv-swe-docs.netlify.app/backend/basic-auth.html) for more
information.

You could add a `Console.Write(users)` in this endpoint to verify that the
password gets hashed and salted.

### IdentityService.cs

Take a look at the `Services/IdentityService.cs` interface and its implementation. `CreateUser` and `AuthenticateUser` create the functionality for the `POST /users` and `GET /users` endpoints.

These methods hash/salt the passwords and store user information (including hashed password) in the data store, and check if the hashed passwords match when a user tries to log in, respectively. These methods are both invoked in the `BasicAuthMiddleware.cs` methods.

### BasicAuthMiddleware.cs

Take a look at the `Middleware/BasicAuthMiddleware.cs` middleware. It parses out the credentials from the auth header and saves them in the `context.Items` object for use by other middleware/controllers.

This basic auth is implemented in the `GET /users` endpoint, which checks the password against the stored value before sending back the user's data. We can say that the `GET /users` endpoint is password protected.

Try accessing it with the header (you need to `POST` this user first!)

```bash
curl -v -XGET \
-H 'Authorization: Basic dGVzdEB1c2VyLmNvbTpwYXNzd29yZDEyMw==' \
'http://localhost:7119/users' | json_pp
```

without the header

```bash
curl -v -XGET \
'http://localhost:7119/users' | json_pp
```

or with the wrong password

```bash
curl -v -XGET \
-H 'Authorization: Basic dGVzdEB1c2VyLmNvbTpwYXNzd29yZDEyNA==' \
'http://localhost:7119/users' | json_pp
```

## Next steps

The apprentices are challenged to implement encryption and basic auth for
themselves. They will likely want to rely on libraries as much as possible: many
frameworks have canonical ways of doing these things which abstract much of the
complexity away. Encourage apprentices to go with the flow of what their
framework recommends. Express.js is very unopinionated so a lot of this feels
very manual, but there are libraries like `passport` which provide abstractions
and are well documented.

Apprentices shouldn't try to memorise what they've seen in the demo, but rather
use the documentation for their framework to implement the spec. Their
particular implementation might look very different and that is fine (encouraged
even!)