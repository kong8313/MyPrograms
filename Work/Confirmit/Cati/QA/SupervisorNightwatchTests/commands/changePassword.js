var events = require('events');
var util = require('util');

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter)

Command.prototype.command = function(oldPassword, newPassword) {
    var self = this;
    this.api.url(function (result) {
      if (result.value.includes("/confirm/authoring/") || result.value.includes("/supervisor/")){
        this.emit("complete");
        return this;
    }

      var changePasswordIdentity = result.value.indexOf('/identity/account/setpassword') > -1;
      var changePassword = result.value.indexOf('/confirm/authoring/ChangePassword.aspx') > -1;
  
      if (changePassword || changePasswordIdentity ) {
        self.api.setValue('#oldPassword', oldPassword)
          .setValue('#newPassword', newPassword)
          .setValue('#confirmPassword', newPassword)
          .click('input[type="submit"]')
          if (changePasswordIdentity) {
            self.api.waitForElementPresent('div.set-password-page input[value = "Save"]', 10000) 
              .click('div.set-password-page input[value = "Save"]');
          }          
      }

      self.emit('complete');
    });
  
    return this;
  };
  
  module.exports = Command;