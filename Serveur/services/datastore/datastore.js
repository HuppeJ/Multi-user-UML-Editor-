// Set up the datastore
const Datastore = require('@google-cloud/datastore');

const keypath = process.cwd() + '/keys/key.json';

// Instantiate a datastore client
const datastore = new Datastore({
  projectId: 'projet-3-228722',
  keyFilename: keypath
});

module.exports = datastore;