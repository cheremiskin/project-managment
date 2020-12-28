import React, { Component } from 'react';
import Home from './pages/Home';
import { ProjectDetail } from './pages/ProjectDetail';
import { Projects } from './pages/Projects';
import './assets/styles/App.css';
import { UserList } from "./pages/UserList";
import { Task } from "./pages/Task";
import { Route } from 'react-router';
import Layout from './components/Layout';
import Profile from './pages/profile/ProfileContainer'

import { connect, Provider } from 'react-redux'
import { applyMiddleware, createStore } from "redux";
import rootReducer from "./store/reducers";
import thunk from "redux-thunk";
import { middleware } from './middleware';
import HttpProvider from './HttpProvider';
import { router } from './router';


const store = createStore(rootReducer, {}, middleware)

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Provider store={store}>
        <Layout>
          <Route exact path='/' component={Home} />
          <Route exact path='/users' component={UserList} />
          <Route path='/user/:id' component={Profile} />
          <Route path='/task/:id' component={Task} />
          <Route path='/projects/' component={Projects} />
          <Route path='/project/:id' component={ProjectDetail} />
        </Layout>
      </Provider>
    );
  }
}
