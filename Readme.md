# TODO
## Global refactoring
- Store sessions (iota, tgs) in Redis/Tarantool/`etc NoSQL`
- Session expiration feature
## TokenGenerationService
- exception handling with middleware (JSON)
## IoTA
- exception handling with middleware (JSON)
- add validation to Entity model
- add Swagger
- `FiwareController` to proxy data to Orion Context Broker with write access control
- `FiwareController` reads data from Orion Context Broker with read access control
- authorization filter for `FiwareController` (use `FiwareService`) **DONE**
## Key Service (for users or machines)
- exception handling with middleware (JSON)
- remove logic from controller to special service
- add Swagger
## Common
- all protocol builder dependencies from Startup files to one extension **DONE**
- create ABE implementation instead mock objects
- create builder for key distribution protocol **DONE**
- nonce serialization/deserialization to/from bytes to specific method in `CryptoHelper`
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
## AbeAuth flow
- TGS get keys through protocol (special service for gettings keys and make abe things) - KeyService Url from settings **DONE**
- inject MockCpAbe to AbeAuthBuilder **DONE**
- send second step from abonent to tgs **DONE**
- sessions in tgs **DONE**
- process second step in tgs **DONE**
- send third step from abonent to tgs **DONE**
- process third step in tgs **DONE**
- send fourth step from abonent to iota **DONE**
- process fourth step **DONE**
- simple request to fiware context broker **DONE**
- send data to iota **DONE**
- proxy data to fiware context broker