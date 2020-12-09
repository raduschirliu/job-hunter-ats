import React from 'react';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom';
import Login from '../Login/Login';
import './App.css';

const App = () => {
  return (
    <Router>
      <div className="app-container">
        <Switch>
          <Route path="/login" component={Login} exact />
        </Switch>
      </div>
    </Router>
  );
};

export default App;
