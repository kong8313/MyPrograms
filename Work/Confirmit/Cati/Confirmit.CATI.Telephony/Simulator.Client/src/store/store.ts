import {Types, Action} from "./actions";
import { Component } from "react";
import { createStore } from 'redux';
import { ActivityInfo } from "../common/api";
import { config } from "../config";

export interface IState{
    url: string;
    activities: ActivityInfo[];
}

const initialState : IState = {
    url: `${location.protocol}//${location.hostname}${location.port ? ':' + location.port : ''}/${config.serviceUrl}`,
    //url: `${location.protocol}//${location.hostname}:3838/${config.serviceUrl}`,
    activities: []
}

function reducer( state = initialState, action: Action  ): IState
{
  switch (action.type) {
    case Types.CONFIGURE:
      return { ... state, url: action.url };
    case Types.SET_ACTIVITIES:
      return { ... state, activities: action.activities };
    default:
      return state;
  }
}


export const store = createStore(reducer); 