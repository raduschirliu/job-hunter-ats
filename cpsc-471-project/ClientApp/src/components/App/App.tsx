import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Companies from '../Companies/Companies';
import Company from '../Company/Company';
import Login from '../Login/Login';
import './App.css';

const ProtectedRoute = ({
  path,
  component,
  exact = false,
}: {
  path: string;
  component: any;
  exact?: boolean;
}) => {
  return <Route path={path} component={component} exact={exact} />;
};

const App = () => {
  return (
    <Router>
      <div className="app-container">
        <Switch>
          <Route path="/login" component={Login} exact />
          <Route path="/companies" component={Companies} exact />
          <Route path="/companies/:companyId" component={Company} exact />
        </Switch>
      </div>
    </Router>
  );
};

export default App;
