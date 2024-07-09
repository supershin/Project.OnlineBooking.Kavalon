using Project.Booking.Services.Data;
using Project.Booking.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Project.Booking.Services.Services.WisePay
{
    public class KPaymentService
    {
        private readonly WisePayEntities _context;
        public KPaymentService()
        {
            _context = new WisePayEntities();
        }

        public async Task SaveUpdateSettled()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await saveUpdateSettledData();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
               
            }
            
        }
        private async Task saveUpdateSettledData()
        {
            //get order charge is success
            var orderCharges = getOrderChargeSuccess();
            //get inquiry from kpayment
            orderCharges = await inquiryTransaction(orderCharges);
            //save order status & order charge transation state  
            saveOrderChargeSettled(orderCharges);
        }
        private List<OrderCharge> getOrderChargeSuccess()
        {
            var query = from c in _context.ts_OrderChagre.Where(e => (e.transaction_state == Constant.WisePay.KBANK.TransactionState.AUTHORIZE
                                        || e.transaction_state == Constant.WisePay.KBANK.TransactionState.CAPTURED)
                                    && e.status == Constant.WisePay.KBANK.Status.SUCCESS)
                        join opm in _context.ts_OrderPaymentMethod
                            on c.OrderPaymentMethodID equals opm.ID
                        join o in _context.ts_Order
                            on opm.OrderID equals o.ID
                        where opm.BankID == Constant.WisePay.BANK.KABNK_ID
                        && o.OrderStatusID == Constant.WisePay.OrderStatus.PAYMENT_SUCCESS
                        select new
                        {
                            CompanyID = o.tm_Project.CompanyID,
                            ChargeID = c.id,
                            OrderID = o.ID,
                            opm.PaymentTypeID,
                            opm.MethodID
                        };
            return query.AsEnumerable().Select(e => new OrderCharge
            {
                CompanyID = e.CompanyID,
                ChargeID = e.ChargeID,
                OrderID = e.OrderID,
                PaymentTypeID = e.PaymentTypeID,
                MethodID = e.MethodID
            }).ToList();
        }
        private async Task<List<OrderCharge>> inquiryTransaction(List<OrderCharge> orderCharges)
        {
            if (orderCharges.Count > 0)
            {
                var api = new WebAPIRest();
                
                foreach (var orderCharge in orderCharges)
                {
                    //get secret key            
                    var secretKey = _context.tr_PaymentGateway.FirstOrDefault(e => e.CompanyID == orderCharge.CompanyID
                                    && e.BankID == Constant.WisePay.BANK.KABNK_ID)?.SecretKey;

                    var obj = new OrderCharge();
                    //for card
                    if (orderCharge.PaymentTypeID == Constant.WisePay.PaymentType.CARD)
                        obj = await inquiry(orderCharge.ChargeID, secretKey);
                    orderCharge.transaction_state = obj.transaction_state;
                }
            }
            return orderCharges;
        }
        private async Task<OrderCharge> inquiry(string chargeID, string skey)
        {
            WebAPIRest api = new WebAPIRest();
            string url = $"{Constant.WisePay.KBANK.GatewayAPI.Url}{Constant.WisePay.KBANK.GatewayAPI.Charge}/{chargeID}";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("x-api-key", skey);

            var result = await api.RequestGet<OrderCharge>(url, headers);

            if (result.Object == "error")
                throw new Exception(result.message);
            return result;
        }
        private void saveOrderChargeSettled(List<OrderCharge> model) {
            foreach (var orderCharge in model)
            {
                //update order
                var order = _context.ts_Order.FirstOrDefault(e => e.ID == orderCharge.OrderID);
                order.IsSettled = (orderCharge.transaction_state == Constant.WisePay.KBANK.TransactionState.SETTLED);
                _context.Entry(order).State = System.Data.Entity.EntityState.Modified;
                
                //update charge
                var charge = _context.ts_OrderChagre.FirstOrDefault(e => e.id == orderCharge.ChargeID);
                charge.transaction_state = orderCharge.transaction_state;
                _context.Entry(charge).State = System.Data.Entity.EntityState.Modified;

            }
            _context.SaveChanges();
        }
    }
}
