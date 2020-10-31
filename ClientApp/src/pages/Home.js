import React, { Component } from 'react';
import RegistrationForm from '../components/RegistrationForm';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <RegistrationForm />
      </div>
    );
  }
}
