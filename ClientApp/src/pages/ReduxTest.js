import React, { Component } from 'react';
import {connect} from 'react-redux';
import {test} from '../actions/test';

class ReduxTest extends Component {
  static displayName = ReduxTest.name;

  render () {
    console.log(this.props);
    return (
      <div>
        <button onClick={this.props.test}>Add</button>
        {/* <RegistrationForm /> */}
        <span>{this.props.count}</span>
      </div>
    );
  }
}

const mapStateToProps = (state) => {
    return {
        count: state.profile.count
    }
}

export default connect(mapStateToProps, {test})(ReduxTest)
