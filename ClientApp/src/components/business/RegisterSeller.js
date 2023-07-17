import React, { Component } from "react";
import TextField from "@mui/material/TextField";
import "../../style/register_restaurant.css";
import { FaPhotoVideo } from "react-icons/fa";
import 'bootstrap/dist/css/bootstrap.css';
import { Services } from "../../utils/services";
import Alert from '@mui/material/Alert';


export class RegisterSeller extends Component {
  constructor(props) {
    super(props);
    this.state = {
      ProfileImage: null,
      ShopName: null,
      PhoneNumber: 0,
      AboutBusiness: null,
      ShopAddress: null,
      profile: null,
      err: false,
      errValue: null,
    };
    this.SubmitFomrm = this.SubmitFomrm.bind(this);
    this.EventInsertion = this.EventInsertion.bind(this);
    this.utils = new Services();
  }

  EventInsertion(ev) {
    this.setState({ [ev.target.name]: ev.target.value });
  }


  PreviewImage(ev, src) {
    const element = ev.target;
    const fakeURL = URL.createObjectURL(element.files[0]);
    this.setState({ [src]: fakeURL });
    this.setState({ ProfileImage: element.files[0] });
  }

  SubmitFomrm(ev) {
    ev.preventDefault();
    const formData = new FormData();
    const value = {
      ShopName: this.state.ShopName,
      PhoneNumber: this.state.PhoneNumber,
      AboutBusiness: this.state.AboutBusiness,
      ShopAddress: this.state.ShopAddress,
    }
    formData.append("stringData", JSON.stringify(value));
    formData.append("image", this.state.ProfileImage);

    fetch(this.utils.getServer() + "/market/RegisterSeller", {
      method: "post",
      headers: {
        'Authorization': 'Bearer ' + this.utils.token()
      },
      body: formData,
    }).then(rsp => rsp.json()).then((res) => {
      if (res.statusCode !== 200) {
        this.setState({ errValue: res.value }, () => {
          this.setState({ err: true });
        })
      } else {
        const value = res.value;
        localStorage.setItem("authToken", value);
      }
    })
  }

  render() {
    return (
      <div> <br /> <br />
        <h6>Register your Shop</h6> <br />
        <div>
          <form action="">
            <TextField onInput={this.EventInsertion} className="input" name="ShopName" label="Shop Name" variant="outlined" />{" "}
            <br /> <br />
            <TextField
              id="outlined-number"
              label="Phone Number 977"
              onAbort={this.EventInsertion}
              type="number"
              name="PhoneNumber"
              className="input"
              InputLabelProps={{
                shrink: true,
              }}
            />
            <br /> <br />
            <label id="profileInputUi">
              {this.state.profile !== null && <div style={{ width: "100%", height: "100%" }}>  <img src={this.state.profile} height="100%" width="100%" /> </div>}
              {this.state.profile === null && <FaPhotoVideo id="mid-icons" />}
              <input onInput={(ev) => { this.PreviewImage(ev, "profile") }} type="file" accept="image/*" style={{ display: "none" }} />
            </label> <br /> <br /> <br /> <br />
            <textarea name="AboutBusiness" onInput={this.EventInsertion} placeholder="About your business..." className="form-control input"></textarea> <br /> <br />
            <TextField name="ShopAddress" onInput={this.EventInsertion} className="input" label="Shop Address" variant="outlined" />{" "}<br /> <br />
            <button onClick={this.SubmitFomrm} className="btn btn-primary input">Submit</button> <br /> 
            {this.state.err && <Alert className="input" style={{height: "50px"}} id="alert" severity="warning">{this.state.errValue}</Alert>}
          </form>
        </div>
      </div>
    );
  }
}
