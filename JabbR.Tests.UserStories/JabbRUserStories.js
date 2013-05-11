var fs = require('fs');

phantom.casperPath = fs.absolute('.');
phantom.injectJs(phantom.casperPath + '\\casperbin\\bootstrap.js');

var hostName = "http://localhost:16207",
invalidUserLoginErrorMessage = "Login failed. Check your username/password.",
registrationPath = '/account/register',
casper = require('casper').create({
	/*clientScripts:  [
        'includes/jquery-1.9.0.min.js' // These two scripts will be injected in remote         'includes/underscore.js'   // DOM on every request
    ],*/
    verbose: true//,	logLevel: "info"
}),
x = require('casper').selectXPath;

casper.start(hostName, function() {
    this.test.assertTitle('JabbR', 'JabbR title is the one expected');
    this.test.assertExists('#logo', 'Jabbr logo exist');
	this.test.assertExists(x('//a[text()="Register"]'), 'register hyper link exists');
    this.test.assertEvalEquals(function() {
        return $('.controls a').attr('href');
    },registrationPath, 'register hyperlink has valid link');

	this.fill("form.form-horizontal",{
		'username':'invaliduser',
		'password':'invalidpassword'
	}, true);
});

casper.then(function(){
	this.test.assertEvalEquals(function() {
        return document.querySelector('ul.validation-summary-errors > li').innerHTML;
    },invalidUserLoginErrorMessage, 'invalid login get expected error message');
	
	// click Register hyperlink
	this.click('.controls a');
	
});

casper.then(function(){
	console.log('Navigated to register screen, new location is ' + this.getCurrentUrl());
	
	this.test.assertExists(x('//button[text()="Register"]'), 'register button exists');
	this.test.assertExists(x('//a[text()="Cancel"]'), 'cancel button exists');

});

casper.run(function() {
    this.test.done(7); // checks that 5 assertions have been executed
    this.test.renderResults(true,0, 'log.xml');
});