import { Button, TextField } from '@material-ui/core';
import React, { useContext, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useHistory } from 'react-router';
import AuthContext from '../../contexts/AuthContext';
import IUserRegistration from '../../models/IUserRegistration';
import './Register.css';

/**
 * Registration component, responsible for taking in form input and
 * sending a request to the server to create a new user, then log them in
 */
const Register = () => {
  const history = useHistory();
  const { register, handleSubmit } = useForm();
  const { register: registerUser, isLoggedIn } = useContext(AuthContext);

  useEffect(() => {
    if (isLoggedIn()) {
      history.push('/companies');
    }
  }, [isLoggedIn, history]);

  const onSubmit = (data: IUserRegistration) => {
    registerUser(data).then(res => {
    }).catch(err => {
      console.log(err);
    });
  };

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleSubmit(onSubmit)}>
        <TextField name="username" label="Username" inputRef={register} />
        <TextField name="password" label="Password" type="password" inputRef={register} />
        <TextField name="firstName" label="First Name" inputRef={register} />
        <TextField name="lastName" label="Last Name" inputRef={register} />
        <Button variant="outlined" color="primary" type="submit">Register</Button>
      </form>
    </div>
  );
};

export default Register;
