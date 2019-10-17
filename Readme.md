# TODO
## AbeAuth flow
- TGS get keys through protocol (special service for gettings keys and make abe things)
- inject MockCpAbe (or encrypted data) to AbeAuthBuilder
- send second step from abonent to tgs
- process second step in tgs 
## TokenGenerationService
- exception handling with middleware (JSON)
## IoTA
- exception handling with middleware (JSON)
- add validation to Entity model
- add Swagger
- `FiwareController` to proxy data to Orion Context Broker with write access control
- `FiwareController` reads data from Orion Context Broker with read access control
- authorization filter for `FiwareController` (use `FiwareService`)
## Key Service (for users or machines)
- exception handling with middleware (JSON)
- remove logic from controller to special service
- add Swagger
## Common
- create ABE implementation instead mock objects
- create builder for key distribution protocol **DONE**
## Attribute Authority
### MVC part
- make `Login.Name` unique (may be check before create)
- add index for `Login.name`
- add minimal length requirement/validation for `Login.name`
- add length check for `Login.SharedKey`, it will be used in `SymmetricEncryption` helper (128 bytes AES key size for example)
- add pretty UI
- add role-based authorization for this app
- remove Login access logic from `HomeController` to special service **DONE**
- add validation in `HomeController` to POST methods
- checking for unique attribures in array for `Create action`
- support for adding attributes to array in `Create view`
- `Edit action` and `Edit page`
- `Sinle page` with key events
- `Delete action` for logins 
- add abonent type to Login model (device or key service)
- generate master keys in singleton object for next key generation operations **DONE**
- save key events on key generation
- exception handling with filter (JSON) for API (`KeysController`)
- pagination in Login list view and `HomeController`
- add Swagger for API