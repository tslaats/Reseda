// @flow
import React from 'react';
import ReactDOM from 'react-dom';
import PropTypes from 'prop-types';
import { BrowserRouter as Router, Route, Redirect } from 'react-router-dom';
import registerServiceWorker from './registerServiceWorker';
import './index.css';
import axios from 'axios';
import { examples } from './examples';


function toClass(name) {
  return name.toLowerCase().replace(' ', '-');
}

function Tile(props) { 
  return (
    <article 
      className={`tile inner is-child notification ${props.classes} ${toClass(props.title)}`}>
        <h1 className="title">{props.title}</h1>
        <h1 className="subtitle">&nbsp;</h1>
        {props.children}
    </article>
  );
}


function Panel(props) { 
  return (
    <div className="tile is-ancestor">
       {props.children}
    </div>
  );
}


Panel.propTypes = {
  title : PropTypes.string.isRequired
}


class Path extends React.Component { 
  constructor(props) { 
    super(props);
    this.state = {
      src: "",
      matches: [],
      msg: null,
    }

    this.onChange = this.onChange.bind(this);
    this.submit = this.submit.bind(this);
  }

  remote () { 
    axios.get(`/api/${this.props.term}/path`, {
      params: {
        expr: this.state.src
      }})
      .then((response) => { 
        this.setState({ 
          matches: response.data,
          msg: null
        });
      })
      .catch( (err) => { 
        this.setState({
          matches: [],
          msg: err.response ? err.response.data : JSON.stringify(err)
        });
      });
  }

  componentDidUpdate(prevProps) { 
    if (this.props.term !== prevProps.term && this.props.src) {
      this.remote()
    }
  }

  onChange(event) { 
    this.setState({
      src: event.target.value
    });

    if (this.timer) { 
      clearTimeout(this.timer);
    }

    this.timer = setTimeout(() => {
      this.remote()
      this.timer = null;
    }, 250);
  }

  submit() { 
    axios.get(`/api/${this.props.term}/exec`, { 
      params: { 
        event: this.state.matches[0].slice(0,-1),
        value: this.input.value
      }})
    .then((response) => { this.props.setTerm( response.data ); })
    .catch( (err) => { 
        this.setState({
          msg: err.response ? err.response.data : JSON.stringify(err)
        });
      });
  }

  render() { 
    const disabled = this.state.matches.length !== 1;
    const matches = this.state.matches.map ((match) => <div key={match}>{match}</div>);
    return (
      <div className="tile is-ancestor">
        <div className="tile is-parent">
          <Tile title="Execution">
            <div className="field is-12 is-grouped">
              <p className="control is-expanded">
                <input 
                  className={ "input" + (this.state.msg ? ' is-danger' : '') } 
                  placeholder="Enter a path expression here" 
                  onChange={this.onChange}
                  value={this.state.src}
                />
                { this.state.msg && 
                  <span className="help is-danger">{this.state.msg}</span>
                }
                { !this.state.msg && disabled &&
                  <span className="help">
                    To execute an event, enter a path expression that
                    matches exactly that event.
                  </span>
                }
                { !this.state.msg && !disabled && 
                  <span className="help">
                    Optionally enter a value for the event, then 
                    click 'Execute'.
                  </span>
                }
              </p>
              <p className="control">
                <input 
                  className="input" 
                  placeholder="Value"
                  type="text" 
                  disabled={disabled} 
                  ref={(input) => {this.input = input;}}/>
              </p>
              <p className="control">
                <button 
                  className="button is-primary" 
                  disabled={disabled}
                  onClick={this.submit}>
                  Execute
                </button>
              </p>
            </div>
            <div className="column match-table">
              { matches }
            </div>
          </Tile>
        </div>
      </div>
    );
  }
} 


class Parser extends React.Component { 
  constructor(props) { 
    super(props);
    this.state = {
      src: "",
      msg: null,
      id: null
    }

    this.onChange = this.onChange.bind(this);
    this.remote();
  }

  remote () { 
    axios.post('/api/parse', this.state.src)
      .then((response) => { 
        this.setState({ 
          id: response.data,
          msg: null
        });
      })
      .catch( (err) => { 
        this.setState({
          msg: err.response ? err.response.data : JSON.stringify(err)
        });
      });
  }

  onChange(event) { 
    this.setState({
      src: event.target.value
    });
  }

  componentDidUpdate(prevProps, prevState) { 
    if (this.state.src !== prevState.src) { 
      if (this.timer) { 
        clearTimeout(this.timer);
      }

      // TODO: May want to cancel request also on onChange?
      this.timer = setTimeout(() => {
        this.remote()
      }, 350);
    }
  }

  render() { 
    const selection = 
      Object.keys(examples).map((key) =>
        <option key={key}>{key}</option>)
    return (
      <div className="tile is-ancestor">
        <div className="tile is-parent">
          <Tile title="Parser" classes="">
            <div className="field">
              <p className="control is-expanded">
                <span className="select is-fullwidth">
                  <select onChange={(evt) => { 
                      this.setState( { src: examples[evt.target.value] });
                    }}>
                    { selection }
                  </select>
                </span>
              </p>
            </div>
            <div className="field">
              <p className="control">
                <textarea 
                  className={ "textarea" + (this.state.msg ? ' is-danger' : '') } 
                  placeholder="Enter source code here" 
                  onChange={this.onChange}
                  value={this.state.src}
                />
                { this.state.msg && 
                  <span className="help is-danger">{this.state.msg}</span>
                }
              </p>
            </div>
            <div className="field is-grouped" style={{justifyContent: 'flex-end'}}>
              <p className="control">
                <button 
                  className="button is-primary" 
                  disabled={this.state.id === null}
                  onClick={() => { this.props.setTerm(this.state.id); }}
                >Load</button>
              </p>
            </div>
          </Tile>
        </div>
      </div>
    );
  }
} 


class Term extends React.Component { 
  constructor(props) { 
    super(props);
    this.state = {
      term: props.term,
      tree: []
    }
  }

  remote () { 
    axios.get(`/api/${this.props.term}/term`)
      .then((response) => { 
        this.setState({ 
          term: response.data['src@'],
          tree: response.data['tree@'].split(/\r?\n/)
        });
      })
      .catch( (err) => { 
        this.setState({
          msg: err.response ? err.response.data : JSON.stringify(err)
        });
      });
  }

  componentDidMount() { 
    this.remote();
  }

  componentDidUpdate(prevProps) { 
    if (prevProps.term !== this.props.term) { 
      this.remote();
    }
  }

  render() { 
    const evts = this.state.tree.map ((evt) => <div key={evt}>{evt}</div>);
    return (
      <div className="tile is-ancestor">
        <div className="tile is-parent">
          <Tile title="Term" classes="is-info">
            { this.state.term }
          </Tile>
        </div>
        <div className="tile is-parent">
          <Tile title="Marking" classes="is-warning">
            { evts }
          </Tile>
        </div>
      </div>
    );
  }
} 



class Analysis extends React.Component { 
  constructor(props) { 
    super(props);

    this.state = { 
      term: null,
      bounded: null,
      live: null
    };

    this.remote = this.remote.bind(this);
  }

  remote (url, f) { 
    axios.get(`/api/${this.props.term}/` + url)
      .then((response) => f(response.data))
      .catch( (err) => { 
        console.log(err.response ? err.response.data : JSON.stringify(err));
      });
  }

  isCurrent() { 
    return this.state.term === this.props.term && this.state.bounded !== null;
  }

  render() { 
    const maybe = (x) => x === null ? "(Not computed)" : (x ? "Yes" : "No");
    return (
      <div className="tile is-ancestor" >
        <div className="tile is-parent">
          <Tile title="Liveness" classes="is-info">
            <p className="subtitle"> 
              Bounded: <br/>{ maybe(this.state.bounded) } 
            </p>
            <p className="subtitle"> 
              Live: <br/>{ maybe(this.state.live) } 
            </p>
            <p>
              { ! this.isCurrent() && this.state.bounded && 
                `NB! You have updated the term. If you used the 
                 parser to load a new term, the analysis is now out 
                 of date.`
              }
            </p>
            <p className="control" style={{textAlign: 'right'}}>
              <button 
             className="button is-primary" 
             disabled={this.isCurrent()}
             onClick={() => this.remote(
               "analysis",
               (data) => { 
                 this.setState ({
                   term: this.props.term,
                   bounded: data['bounded@'],
                   live: data['live@']
                 })})}>
              Analyse
             </button>
           </p>
        </Tile>
      </div>
      <div className="tile is-parent">
        <Tile title="Input serialisation" classes="is-primary">
          <p> Click the button below to add relations to ensure strict
            serialisation of input- and output events (cf. Definition ?.?). 
          </p>
          <br/>
          <div className="field">
            <p className="control" style={{textAlign: 'right'}}>
              <button
                className="button is-danger"
                onClick={() => this.remote(
                  "antipar", (term) => { this.props.setTerm(term); }
                )}>
                Serialise
              </button>
            </p>
          </div>
        </Tile>
      </div>
      <div className="tile is-parent">
        <Tile title="Glitch freedom" classes="is-primary">
          <p> Click the button below to automatically add
            relations to ensure glitch-freedom (cf. Definition ?.?). 
          </p>
          <br/>
          <div className="field">
            <p className="control" style={{textAlign: 'right'}}>
              <button
                className="button is-danger"
                onClick={() => this.remote(
                  "antiglitch", (term) => { this.props.setTerm(term); }
                )}>
                Anti-glitch
              </button>
            </p>
          </div>
        </Tile>
      </div>
    </div>
    );
  }
} 


const App = (props) => {
    const cxt = {
      term: props.match.params.term,
      setTerm: (id) => { 
        props.history.push(`/process/${id}`);
      }
    }
    return (
        <div>
          <Term {...cxt}/>
          <Path {...cxt}/>
          <Analysis {...cxt}/>
          <Parser {...cxt}/>
        </div>
    );
  };


ReactDOM.render(
  <Router>
    <div>
      <Route path="/process/:term" component={App} />
      <Route exact path="/" render={() => (
        <Redirect to="/process/2714325035"/>
      )}/>
    </div>
  </Router>,
  document.getElementById('root')
);

registerServiceWorker();



