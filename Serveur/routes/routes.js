'use strict';

var controller = require('./controllers');

module.exports = (app) => {
    app.route('/')
        .get(controller.home);
    app.route('/about')
        .get(controller.about);
    app.route('/distance/:zipcode1/:zipcode2')
        .get(controller.get_distance);
};