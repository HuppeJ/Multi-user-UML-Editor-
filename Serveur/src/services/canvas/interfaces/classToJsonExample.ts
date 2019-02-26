// See : http://choly.ca/post/typescript-json/

interface AccountJSON {
    name: string;
}


class Account {
    public name: string;

    // toJSON is automatically used by JSON.stringify
    toJSON(): AccountJSON {
        // copy all fields from `this` to an empty object and return in
        return Object.assign({}, this);
    }

    // fromJSON is used to convert an serialized version
    // of the User to an instance of the class
    static fromJSON(json: AccountJSON | string): User {
        if (typeof json === 'string') {
            // if it's a string, parse it first
            return JSON.parse(json, Account.reviver);
        } else {
            // create an instance of the User class
            let account = Object.create(Account.prototype);
            // copy all the fields from the json object
            return Object.assign(account, json,);
        }
    }

    // reviver can be passed as the second parameter to JSON.parse
    // to automatically call User.fromJSON on the resulting value.
    static reviver(key: string, value: any): any {
        return key === "" ? User.fromJSON(value) : value;
    }
}

// A representation of User's data that can be converted to
// and from JSON without being altered.
interface UserJSON {
    name: string;
    age: number;
    created: string;
    account: Account
}

class User {

    public created: Date;
    public account: Account;

    constructor(
        public name: string,
        public age: number
    ) {
        this.created = new Date();
    }

    // example
    getName(): string {
        return this.name;
    }

    // toJSON is automatically used by JSON.stringify
    toJSON(): UserJSON {
        // copy all fields from `this` to an empty object and return in
        return Object.assign({}, this, {
            // convert fields that need converting
            created: this.created.toString(),
        });
    }

    // fromJSON is used to convert an serialized version
    // of the User to an instance of the class
    static fromJSON(json: UserJSON | string): User {
        if (typeof json === 'string') {
            // if it's a string, parse it first
            return JSON.parse(json, User.reviver);
        } else {
            // create an instance of the User class
            let user = Object.create(User.prototype);
            // copy all the fields from the json object
            return Object.assign(user, json, {
                // convert fields that need converting
                created: new Date(json.created),
                account: Account.fromJSON(json.account)
            });
        }
    }

    // reviver can be passed as the second parameter to JSON.parse
    // to automatically call User.fromJSON on the resulting value.
    static reviver(key: string, value: any): any {
        return key === "" ? User.fromJSON(value) : value;
    }
}

