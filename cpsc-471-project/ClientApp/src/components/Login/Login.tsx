import { Button, TextField } from '@material-ui/core';
import React, { useContext } from 'react';
import { useForm } from 'react-hook-form';
import { useHistory } from 'react-router';
import AuthContext from '../../contexts/AuthContext';
import './Login.css';

const Login = () => {
  const history = useHistory();
  const { register, handleSubmit } = useForm();
  const { login } = useContext(AuthContext);

  const onSubmit = (data: any) => {
    login(data.username, data.password).then(res => {
      history.push('/companies');
    }).catch(err => {
      console.log(err);
    });
  };

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleSubmit(onSubmit)}>
        <TextField name="username" label="Username" inputRef={register} />
        <TextField name="password" label="Password" type="password" inputRef={register} />
        <Button variant="outlined" color="primary" type="submit">Login</Button>
      </form>
    </div>
  );
};

export default Login;