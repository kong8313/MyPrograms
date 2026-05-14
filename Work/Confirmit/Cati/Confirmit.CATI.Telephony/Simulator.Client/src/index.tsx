import * as React from 'react'
import { render } from 'react-dom'
import { Provider } from 'react-redux';
import {store} from "./store";
import App from './views/app';
import { BrowserRouter as Router, Route, Switch} from 'react-router-dom';

render(
    <Router>
        <Provider store={store}>
            <Switch>
                <Route path="/" component={App} />
            </Switch>
        </Provider>
    </Router>, 
    document.getElementById('root'))