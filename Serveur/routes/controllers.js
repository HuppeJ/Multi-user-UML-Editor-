'use strict';

var properties = require('../package.json')

var controllers = {
    home: (req, res) => {
        res
            .status(200)
            .send('Hello, world!!')
            .end();
    },
    about: (req, res) => {
        var aboutInfo = {
            name: properties.name,
            version: properties.version
        }
        res.json(aboutInfo);
    },
    get_distance: (req, res) => {
        
    },
};

module.exports = controllers;

// On pourrait utiliser un syntax comme celle-ci ça serait plus clean pour les imports
// /**
//  * GET /contact
//  * Contact form page.
//  */
// exports.getContact = (req, res) => {
//     const unknownUser = !(req.user);
  
//     res.render('contact', {
//       title: 'Contact',
//       unknownUser,
//     });
//   };
