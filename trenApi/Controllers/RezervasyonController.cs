using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trenApi.Model;

namespace trenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RezervasyonController : ControllerBase
    {
        List<Tren> _trenler;
        public RezervasyonController()
        {
            _trenler = new List<Tren>()
            {
                new Tren()
                {
                    Ad = "Başkent Ekspres",
                    Vagonlar = new List<Vagon>()
                    {
                        new Vagon()
                        {
                             Ad ="Vagon 1",
                             Kapasite=100,
                             DoluKoltukAdet=50
                        },
                        new Vagon()
                        {
                             Ad ="Vagon 2",
                             Kapasite=90,
                             DoluKoltukAdet=80
                        },
                         new Vagon()
                         {
                             Ad ="Vagon 3",
                             Kapasite=80,
                             DoluKoltukAdet=80
                         }
                    }
                },
                new Tren()
                {
                    Ad = "Doğu Express",
                    Vagonlar = new List<Vagon>()
                    {
                        new Vagon()
                        {
                             Ad ="Vagon 1",
                             Kapasite=180,
                             DoluKoltukAdet=150
                        },
                        new Vagon()
                        {
                             Ad ="Vagon 2",
                             Kapasite=170,
                             DoluKoltukAdet=50
                        },
                         new Vagon()
                         {
                             Ad ="Vagon 3",
                             Kapasite=150,
                             DoluKoltukAdet=100
                         }
                    }
                }
            };
        }

        [HttpGet("GetTrainInformation")]
        public IActionResult GetTrainInformation()
        {
            return Ok(_trenler);
        }

        [HttpPost("SetReservation")]
        public IActionResult SetReservation(Input input)
        {
            List<Vagon> KapasiteliVagonlar = input.Tren.Vagonlar.Where(x => x.Kapasite * 0.70 > x.DoluKoltukAdet).ToList();


            if (KapasiteliVagonlar.Count > 0 && KapasiteliVagonlar.Select(x=>x.Kapasite-x.DoluKoltukAdet).Sum() >= input.RezervasyonYapilacakKisiSayisi && input.KisilerFarkliVagonlaraYerlestirilebilir == true)
            {
                List<YerlesimAyrinti> yerlesimAyrintilari = new List<YerlesimAyrinti>();

                int kisiSayisi = input.RezervasyonYapilacakKisiSayisi;

                foreach (Vagon Vagon in KapasiteliVagonlar)
                {
                    int Kapasite = Vagon.Kapasite - Vagon.DoluKoltukAdet;
               
                    YerlesimAyrinti yerlesimAyrintisi = new YerlesimAyrinti()
                    {
                        VagonAdi = Vagon.Ad,
                        KisiSayisi = kisiSayisi >= Kapasite ? Math.Abs(Kapasite) : kisiSayisi
                    };
                    yerlesimAyrintilari.Add(yerlesimAyrintisi);

                    kisiSayisi -= Kapasite;

                    if (kisiSayisi <= 0)
                        break;
                }

                return Ok(
                        new Output()
                        {
                            RezervasyonYapilabilir = true,
                            YerlesimAyrinti = yerlesimAyrintilari
                        }
                    );
            }
            else if (KapasiteliVagonlar.Count > 0 && KapasiteliVagonlar.Any(x=>x.Kapasite-x.DoluKoltukAdet >= input.RezervasyonYapilacakKisiSayisi) && input.KisilerFarkliVagonlaraYerlestirilebilir == false)
            {

                Vagon vagon = KapasiteliVagonlar.Where(x => x.Kapasite - x.DoluKoltukAdet >= input.RezervasyonYapilacakKisiSayisi).FirstOrDefault();


                return Ok(
                         new Output()
                         {
                             RezervasyonYapilabilir = true,
                             YerlesimAyrinti = new List<YerlesimAyrinti>()
                             {
                                new YerlesimAyrinti()
                                    {
                                        VagonAdi = vagon.Ad,
                                        KisiSayisi = input.RezervasyonYapilacakKisiSayisi
                                    }
                            }
                         }
                    );
            }
            else
            {
                return Ok(
                        new Output()
                        {
                            RezervasyonYapilabilir=false,
                            YerlesimAyrinti = null
                        }
                    );
            }
        }
    }
}
