import React, { Component } from 'react';
import {SearchBar} from './searchbar';
export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <SearchBar/>
        <br></br>
      </div>
    );
  }
}
