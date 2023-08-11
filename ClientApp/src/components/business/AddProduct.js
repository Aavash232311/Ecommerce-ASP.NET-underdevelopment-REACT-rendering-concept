import React, { Component } from "react";
import { SellerDashboard } from "./SellerDashboard";
import "../../style/add_product.css";
import { CKEditor } from '@ckeditor/ckeditor5-react';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { Services } from "../../utils/services";
import { Category } from "../Admin/AdminCat";

export class AddProduct extends Component {
    constructor(props) {
        super(props);
        this.state = {
            discount: false,
            description: null,
            name: null,
            price: 0,
            discount_amount: 0,
            discount_valid_date: "",
            related_tags: null,
            main_image: null,
            cover_0: null,
            cover_1: null,
            cover_2: null,
            cover_3: null,
            cover_4: null,
            product_condition: "New",
            success: false,
            reverse_engineer: props.reverse_engineer ? true : false,
            view_img: null,
        }
        this.utils = new Services();
        this.AddProductState = this.AddProductState.bind(this);
        this.FormSubmit = this.FormSubmit.bind(this);
        this.componentDidMount = this.componentDidMount.bind(this);
    }

    componentDidMount() {
        // in case of re populating data
        if (this.state.reverse_engineer === true) {
            const populateData = this.props.populateData[0];
            let temp = [];
            for (let i in populateData) {
                this.setState({ [i]: populateData[i] });
            }
        }
    }

    AddProductState(ev) {
        if (ev.target.type === "text" || ev.target.type === "number" || ev.target.type === "date" || ev.target.type === "select-one") {
            this.setState({ [ev.target.name]: ev.target.type === "number" ? parseInt(ev.target.value) : ev.target.value }, () => {
             
            });
            return;
        }
        if (ev.target.type === "file") {
            this.setState({ [ev.target.name]: ev.target.files[0] })
        }
    }

    FormSubmit(ev) {
        ev.preventDefault();
        const formData = new FormData();
        const submitObject = {
            product_condition: this.state.product_condition,
            description: this.state.description,
            name: this.state.name,
            price: this.state.price,
            discount_amount: this.state.discount_amount,
            discount_valid_date: this.state.discount_valid_date,
            related_tags: this.state.related_tags.id,
            type: this.state.reverse_engineer ?  this.props.populateData[0].id: "",
        }
        const stringObj = JSON.stringify(submitObject);
        formData.append("key", stringObj);
        
        if (this.state.main_image == null) {
            alert("Atleast one image is required");
        }
        formData.append("main_image", this.state.main_image);
        formData.append("cover_0", this.state.cover_0);
        formData.append("cover_1", this.state.cover_1);
        formData.append("cover_2", this.state.cover_2);
        formData.append("cover_3", this.state.cover_3);
        formData.append("cover_4", this.state.cover_4);
        fetch(this.utils.getServer() + "/market/add_product_form110", {
            method: "post",
            headers: {
                'Authorization': 'Bearer ' + this.utils.token()
            },
            body: formData,
        }).then(rsp => rsp.json()).then((res) => {
            if (res.statusCode === 200) {
                // set object of each and every value from update
                for (const  i in res.value) {
                    this.setState({[i]: res.value[i]});
                }
                this.setState({ success: true });
                window.location.reload();
            }
        })
    }



    render() {
        const editorConfig = {
            toolbar: {
                items: [
                    'undo', 'redo',
                    '|', 'heading',
                    '|', 'fontfamily', 'fontsize', 'fontColor', 'fontBackgroundColor',
                    '|', 'bold', 'italic', 'strikethrough', 'subscript', 'superscript', 'code',
                    '|', 'link', 'blockQuote', 'codeBlock',
                    '|', 'bulletedList', 'numberedList', 'todoList', 'outdent', 'indent'
                ],
            }
        };
        const dirUrlToPath = (url) => {
            if (url != null || url !== "") {
                url = url.replace(/\\/g, "/");
                return this.utils.getServer() + "/" + url;
            }
        }
        return (
            <div>
                {this.state.reverse_engineer ? null : <SellerDashboard />}
                {this.state.view_img ? (<center>
                    <div className="shadow-sm p-3 mb-5 bg-white rounded viewImageFrame">
                        <span style={{ float: "left", cursor: "pointer" }} onClick={() => { this.setState({ view_img: null }) }}><b>X</b></span>
                        <img src={this.state.view_img} height='auto' width="auto" />
                    </div>
                </center>) : null}
                <form>
                    <br/>
                    <input onInput={(ev) => { this.AddProductState(ev) }} value={this.state.name ? this.state.name : ""} required className="input form-control" placeholder="Product Title" name="name" /> <br />
                    <input onInput={(ev) => { this.AddProductState(ev) }} value={this.state.price ? this.state.price : ""} required className="input form-control" placeholder="Price" name="price" type="number" />
                    <div className="input">
                        <span className="label">
                            Remember to add thing like product, details descrpitions etc..
                        </span> <br />
                        <CKEditor data={this.state.description ? this.state.description : ""} onChange={(event, editor) => {
                            const data = editor.getData();
                            this.setState({ description: data });
                        }} editor={ClassicEditor} config={editorConfig} />
                    </div>
                    <div className="form-check input">
                        <input checked={this.state.discount_amount > 0 ? true : false} onInput={() => { this.state.discount ? this.setState({ discount: false }) : this.setState({ discount: true }); this.setState({ discount_amount: 0 }) }} className="form-check-input" type="checkbox" value="" id="flexCheckIndeterminate" />
                        <label className="form-check-label" htmlFor="flexCheckIndeterminate">
                            Discount?
                        </label>
                    </div>
                    <div style={{ display: this.state.discount || this.state.discount_amount > 0 ? "block" : "none" }}>
                        <input value={this.state.discount_amount ? this.state.discount_amount : ""} onInput={(ev) => { this.AddProductState(ev) }} type="number" placeholder="discount %" className="input form-control" name="discount_amount" /> <br />
                        <span className="label">
                            Discount Valid Till
                        </span>
                        <br />
                        <input value={this.state.discount_valid_date ? this.state.discount_valid_date.slice(0, 10) : ""} onInput={(ev) => { this.AddProductState(ev) }} type="date" name="discount_valid_date" className="input form-control" />
                        {/* <a href={dirUrlToPath(this.state.main_image)}>view</a>  */}
                        {/* <input   {...(this.state.reverse_engineer && this.state.discount_valid_date ? { value: '2023-03-06' } : {})} onInput={(ev) => { this.AddProductState(ev) }} type="date" name="discount_valid_date" className="input form-control" /> */}
                    </div> <br />
                    <div className="input alignbigDiv">
                        {this.state.reverse_engineer && this.state.related_tags !== null ? (<div>
                           Selected: <b>{this.state.related_tags.productCategory}</b>
                        </div>) : null}
                        <Category className="input" onChange={(ev) => {this.setState({related_tags: ev})} }  />
                    </div>
                    <br />
                    <span className="label">
                        Main Image less that 5mb use high resulotion for sharpest image
                    </span> <br />
                    <input required type="file" onInput={(ev) => { this.AddProductState(ev) }} className="form-control input" accept="image/*" name="main_image" />
                    <span onClick={() => {
                        this.setState({ view_img: dirUrlToPath(this.state.main_image) });
                    }} className="viewImage">view image</span>
                    <br />
                    <span className="label">
                        Product Image less that 5mb use high resulotion for sharpest image
                    </span> <br />
                    <input type="file" onInput={(ev) => { this.AddProductState(ev) }} className="form-control input" accept="image/*" name="cover_0" /> <br />
                    {this.state.reverse_engineer && this.state.cover_0 ? (
                        <span onClick={() => {
                            this.setState({ view_img: dirUrlToPath(this.state.cover_0) })
                        }} className="viewImage">view image</span>
                    ) : null}
                    <input type="file" onInput={(ev) => { this.AddProductState(ev) }} className="form-control input" accept="image/*" name="cover_1" /> <br />
                    {this.state.reverse_engineer && this.state.cover_1 ? (
                        <span onClick={() => {
                            this.setState({ view_img: dirUrlToPath(this.state.cover_1) })
                        }} className="viewImage">view image</span>
                    ) : null}
                    <input type="file" onInput={(ev) => { this.AddProductState(ev) }} className="form-control input" accept="image/*" name="cover_2" /> <br />
                    {this.state.reverse_engineer && this.state.cover_2 ? (
                        <span onClick={() => {
                            this.setState({ view_img: dirUrlToPath(this.state.cover_2) })
                        }} className="viewImage">view image</span>
                    ) : null}
                    <input type="file" onInput={(ev) => { this.AddProductState(ev) }} className="form-control input" accept="image/*" name="cover_3" /> <br />
                    {this.state.reverse_engineer && this.state.cover_3 ? (
                        <span onClick={() => {
                            this.setState({ view_img: dirUrlToPath(this.state.cover_3) })
                        }} className="viewImage">view image</span>
                    ) : null}
                    <input type="file" onInput={(ev) => { this.AddProductState(ev) }} className="form-control input" accept="image/*" name="cover_4" /> <br />
                    {this.state.reverse_engineer && this.state.cover_4 ? (
                        <span onClick={() => {
                            this.setState({ view_img: dirUrlToPath(this.state.cover_4) })
                        }} className="viewImage">view image</span>
                    ) : null}
                    <span className="label">
                        Select product condition
                    </span> <br />
                    <select onChange={(ev) => {this.AddProductState(ev) } } value={this.state.reverse_engineer ? this.state.product_condition : null} name="product_condition" required className="form-control input">
                        <option value="new">New</option>
                        <option value="brand_new">Brand new</option>
                        <option value="just_like_new">Just like new</option>
                        <option value="broken">Broken</option>
                    </select>
                    <br/>
                    <button onClick={(ev) => { this.FormSubmit(ev) }} className="input btn btn-success">Add Product</button>
                    <br/>
                    {this.state.success && <div className="alert alert-success input" role="alert">
                        Product Added Successfully { }
                    </div>}
                </form>
            </div>
        )
    }
}