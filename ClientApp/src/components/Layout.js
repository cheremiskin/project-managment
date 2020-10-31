import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { Button } from 'antd';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <NavMenu />
        <Button type="primary">Primary Button</Button>
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
