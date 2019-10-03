# TODO
## Common
- create ABE implementation instead mock objects
- create builder for key distribution protocol
## Attribute Authority
### MVC part
- make `Login.Name` unique (may be check before create)
- add index for `Login.name`
- add minimal length requirement/validation for `Login.name`
- add length check for `Login.SharedKey`, it will be used in `SymmetricEncryption` helper (128 bytes AES key size for example)
- add pretty UI
- add role-based authorization for this app
- remove Login access logic from `HomeController` to special service
- add validation in `HomeController` to POST methods
- checking for unique attribures in array for `Create action`
- support for adding attributes to array in `Create view`
- `Edit action` and `Edit page`
- `Sinle page` with key events
- `Delete action` for logins 
- Add abonent type to Login model (device or key service)
- On start generate master keys in singlethon object for next key generation operations