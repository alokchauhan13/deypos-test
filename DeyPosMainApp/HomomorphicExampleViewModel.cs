using Common.UI;
using System;
using Microsoft.Research.SEAL;
using System.Text;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class HomomorphicExampleViewModel : ObservableObject
    {


        public DelegateCommand evaluateCommand;

        public DelegateCommand EvaluateCommand
        {
            get
            {
                if (this.evaluateCommand == null)
                {
                    this.evaluateCommand = new DelegateCommand(ExecuteEvaluateCommand, CanExecuteEvaluateCommand);

                }
                return this.evaluateCommand;
            }
        }


        private int value1;

        public int Value1
        {
            get { return this.value1; }
            set
            {
                if (this.value1 != value)
                {
                    this.value1 = value;
                    RaisePropertyChanged(() => Value1);
                }
            }
        }

        private int value2;

        public int Value2
        {
            get { return this.value2; }
            set
            {
                if (this.value2 != value)
                {
                    this.value2 = value;
                    RaisePropertyChanged(() => Value2);
                }
            }
        }

        private string logs;

        public string Logs
        {
            get { return this.logs; }
            set
            {
                if (this.logs != value)
                {
                    this.logs = value;
                    RaisePropertyChanged(() => Logs);
                }
            }
        }


        private void ExecuteEvaluateCommand(Object data)
        {
            ExampleBasics(Value1, Value2);
        }

        private bool CanExecuteEvaluateCommand(Object data)
        {
            return true;
        }


        private void ExampleBasics(int val1, int val2)
        {
            /*
            In this example we demonstrate using some of the basic arithmetic operations on integers.

            SEAL uses the Fan-Vercauteren (FV) homomorphic encryption scheme. We refer to
            https://eprint.iacr.org/2012/144 for full details on how the FV scheme works.
            */

            // Create encryption parameters.
            var parms = new EncryptionParameters();

            /*
            First choose the polynomial modulus. This must be a power-of-2 cyclotomic polynomial,
            i.e. a polynomial of the form "1x^(power-of-2) + 1". We recommend using polynomials of
            degree at least 1024.
            */
            parms.SetPolyModulus("1x^2048 + 1");

            /*
            Next we choose the coefficient modulus. SEAL comes with default values for the coefficient
            modulus for some of the most reasonable choices of PolyModulus. They are as follows:

            /----------------------------------------------------------------------\
            | PolyModulus  | default CoeffModulus                       | security |
            | -------------|--------------------------------------------|----------|
            | 1x^2048 + 1  | 2^60 - 2^14 + 1 (60 bits)                  | 119 bit  |
            | 1x^4096 + 1  | 2^116 - 2^18 + 1 (116 bits)                | 122 bit  |
            | 1x^8192 + 1  | 2^226 - 2^26 + 1 (226 bits)                | 124 bit  |
            | 1x^16384 + 1 | 2^435 - 2^33 + 1 (435 bits)                | 130 bit  |
            | 1x^32768 + 1 | 2^889 - 2^54 - 2^53 - 2^52 + 1 (889 bits)  | 127 bit  |
            \----------------------------------------------------------------------/

            These can be conveniently accessed using ChooserEvaluator.DefaultParameterOptions, which returns 
            the above list of options as a Dictionary, keyed by the degree of the polynomial modulus. The security 
            levels are estimated based on https://eprint.iacr.org/2015/046 and https://eprint.iacr.org/2017/047. 
            We strongly recommend that the user consult an expert in the security of RLWE-based cryptography to 
            estimate the security of a particular choice of parameters.

            The user can also easily choose their custom coefficient modulus. For best performance, it should 
            be a prime of the form 2^A - B, where B is congruent to 1 modulo 2*degree(PolyModulus), and as small 
            as possible. Roughly speaking, When the rest of the parameters are held fixed, increasing CoeffModulus
            decreases the security level. Thus we would not recommend using a value for CoeffModulus much larger 
            than those listed above (the defaults). In general, we highly recommend the user to consult with an expert 
            in the security of RLWE-based cryptography when selecting their parameters to ensure an appropriate level 
            of security.

            The size of CoeffModulus affects the total noise budget that a freshly encrypted ciphertext has. More 
            precisely, every ciphertext starts with a certain amount of noise budget, which is consumed in homomorphic
            operations - in particular in multiplication. Once the noise budget reaches 0, the ciphertext becomes 
            impossible to decrypt. The total noise budget in a freshly encrypted ciphertext is very roughly given by 
            log2(CoeffModulus/PlainModulus), so increasing coeff_modulus will allow the user to perform more
            homomorphic operations on the ciphertexts without corrupting them. However, we must again warn that
            increasing CoeffModulus has a strong negative effect on the security level.
            */
            parms.SetCoeffModulus(ChooserEvaluator.DefaultParameterOptions[2048]);

            /*
            Now we set the plaintext modulus. This can be any positive integer, even though here we take it to be a 
            power of two. A larger plaintext modulus causes the noise to grow faster in homomorphic multiplication, 
            and also lowers the maximum amount of noise in ciphertexts that the system can tolerate (see above).
            On the other hand, a larger plaintext modulus typically allows for better homomorphic integer arithmetic,
            although this depends strongly on which encoder is used to encode integers into plaintext polynomials.
            */
            parms.SetPlainModulus(1 << 8);

            /*
            Once all parameters are set, we need to call EncryptionParameters::validate(), which evaluates the
            properties of the parameters, their validity for homomorphic encryption, and performs some important
            pre-computation.
            */
            parms.Validate();

            /*
            Plaintext elements in the FV scheme are polynomials (represented by the Plaintext class) with coefficients 
            integers modulo PlainModulus. To encrypt for example integers instead, one must use an "encoding scheme", 
            i.e. a specific way of representing integers as such polynomials. SEAL comes with a few basic encoders:

            IntegerEncoder:
            Given an integer base b, encodes integers as plaintext polynomials in the following way. First, a base-b
            expansion of the integer is computed. This expansion uses a "balanced" set of representatives of integers
            modulo b as the coefficients. Namely, when b is off the coefficients are integers between -(b-1)/2 and
            (b-1)/2. When b is even, the integers are between -b/2 and (b-1)/2, except when b is two and the usual
            binary expansion is used (coefficients 0 and 1). Decoding amounts to evaluating the polynomial at x=b.
            For example, if b=2, the integer 26 = 2^4 + 2^3 + 2^1 is encoded as the polynomial 1x^4 + 1x^3 + 1x^1.
            When b=3, 26 = 3^3 - 3^0 is encoded as the polynomial 1x^3 - 1. In reality, coefficients of polynomials
            are always unsigned integers, and in this case are stored as their smallest non-negative representatives
            modulo plain_modulus. To create an integer encoder with a base b, use IntegerEncoder(PlainModulus, b). 
            If no b is given to the constructor, the default value of b=2 is used.

            FractionalEncoder:
            Encodes fixed-precision rational numbers as follows. First expand the number in a given base b, possibly 
            truncating an infinite fractional part to finite precision, e.g. 26.75 = 2^4 + 2^3 + 2^1 + 2^(-1) + 2^(-2)
            when b=2. For the sake of the example, suppose PolyModulus is 1x^1024 + 1. Next represent the integer part 
            of the number in the same way as in IntegerEncoder (with b=2 here). Finally, represent the fractional part 
            in the leading coefficients of the polynomial, but when doing so invert the signs of the coefficients. So 
            in this example we would represent 26.75 as the polynomial -1x^1023 - 1x^1022 + 1x^4 + 1x^3 + 1x^1. The 
            negative coefficients of the polynomial will again be represented as their negatives modulo PlainModulus.

            PolyCRTBuilder:
            If PolyModulus is 1x^N + 1, PolyCRTBuilder allows "batching" of N plaintext integers modulo plain_modulus 
            into one plaintext polynomial, where homomorphic operations can be carried out very efficiently in a SIMD 
            manner by operating on such a "composed" plaintext or ciphertext polynomials. For full details on this very
            powerful technique we recommend https://eprint.iacr.org/2012/565.pdf and https://eprint.iacr.org/2011/133.

            A crucial fact to understand is that when homomorphic operations are performed on ciphertexts, they will
            carry over to the underlying plaintexts, and as a result of additions and multiplications the coefficients
            in the plaintext polynomials will increase from what they originally were in freshly encoded polynomials.
            This becomes a problem when the coefficients reach the size of PlainModulus, in which case they will get
            automatically reduced modulo PlainModulus, and might render the underlying plaintext polynomial impossible
            to be correctly decoded back into an integer or rational number. Therefore, it is typically crucial to
            have a good sense of how large the coefficients will grow in the underlying plaintext polynomials when
            homomorphic computations are carried out on the ciphertexts, and make sure that PlainModulus is chosen to
            be at least as large as this number.

            Here we choose to create an IntegerEncoder with base b=2.
            */
            var encoder = new IntegerEncoder(parms.PlainModulus);

            // Encode two integers as polynomials.
            int value1 = val1;
            int value2 = val2;
            var encoded1 = encoder.Encode(value1);
            var encoded2 = encoder.Encode(value2);

            StringBuilder stBlder = new StringBuilder();

            Logs = string.Empty;

            stBlder.AppendLine(String.Format("Encoded {0} as polynomial {1}", value1, encoded1));
          
            stBlder.AppendLine(String.Format("Encoded {0} as polynomial {1}", value2, encoded2));


            // Generate keys.

            stBlder.AppendLine("Generating keys ...");

           
            var generator = new KeyGenerator(parms);
            generator.Generate();

            stBlder.AppendLine("... key generation completed");

            var publicKey = generator.PublicKey;
            var secretKey = generator.SecretKey;


            PrintBigPolyArray(stBlder, "Public Key : ", publicKey);

            stBlder.AppendLine("");

            stBlder.AppendLine(String.Format("Secret Key : {0}", secretKey));

            stBlder.AppendLine("Encrypting values...");

            var encryptor = new Encryptor(parms, publicKey);
            var encrypted1 = encryptor.Encrypt(encoded1);
            var encrypted2 = encryptor.Encrypt(encoded2);

            PrintBigPolyArray(stBlder, "Encrypted Value 1 : ", encrypted1);
            PrintBigPolyArray(stBlder, "Encrypted Value 2 : ", encrypted2);

            stBlder.AppendLine("Performing arithmetic on ecrypted numbers ...");

            var evaluator = new Evaluator(parms);

            stBlder.AppendLine("Performing homomorphic negation ...");
            var encryptedNegated1 = evaluator.Negate(encrypted1);

            PrintBigPolyArray(stBlder, "Encrypted negate number : ", encryptedNegated1);

            stBlder.AppendLine("Performing homomorphic addition ...");
            var encryptedSum = evaluator.Add(encrypted1, encrypted2);

            PrintBigPolyArray(stBlder, "Encrypted addition number : ", encryptedSum);

            stBlder.AppendLine("Performing homomorphic subtraction ...");
            var encryptedDiff = evaluator.Sub(encrypted1, encrypted2);

            PrintBigPolyArray(stBlder, "Encrypted subtraction number : ", encryptedDiff);

            stBlder.AppendLine("Performing homomorphic multiplication ...");
            var encryptedProduct = evaluator.Multiply(encrypted1, encrypted2);

            PrintBigPolyArray(stBlder, "Encrypted Multiplied number : ", encryptedProduct);
            // Decrypt results.

            stBlder.AppendLine("Decrypting results ...");
            var decryptor = new Decryptor(parms, secretKey);
            var decrypted1 = decryptor.Decrypt(encrypted1);
            var decrypted2 = decryptor.Decrypt(encrypted2);
            var decryptedNegated1 = decryptor.Decrypt(encryptedNegated1);
            var decryptedSum = decryptor.Decrypt(encryptedSum);
            var decryptedDiff = decryptor.Decrypt(encryptedDiff);
            var decryptedProduct = decryptor.Decrypt(encryptedProduct);

            // Decode results.
            var decoded1 = encoder.DecodeInt32(decrypted1);
            var decoded2 = encoder.DecodeInt32(decrypted2);
            var decodedNegated1 = encoder.DecodeInt32(decryptedNegated1);
            var decodedSum = encoder.DecodeInt32(decryptedSum);
            var decodedDiff = encoder.DecodeInt32(decryptedDiff);
            var decodedProduct = encoder.DecodeInt32(decryptedProduct);

            // Display results.

            stBlder.AppendLine(String.Format("Original = {0}; after encryption/decryption = {1}", value1, decoded1));
            stBlder.AppendLine(String.Format("Original = {0}; after encryption/decryption = {1}", value2, decoded2));

            stBlder.AppendLine(String.Format("Encrypted negate of {0} = {1}", value1, decodedNegated1));

            stBlder.AppendLine(String.Format("Encrypted addition of {0} and {1} = {2}", value1, value2, decodedSum));

            stBlder.AppendLine(String.Format("Encrypted subtraction of {0} and {1} = {2}", value1, value2, decodedDiff));

            stBlder.AppendLine(String.Format("Encrypted multiplication of {0} and {1} = {2}", value1, value2, decodedProduct));

            Logs = stBlder.ToString();

            // How much noise budget did we use in these operations?
            Console.WriteLine("Noise budget in encryption of {0}: {1} bits", value1, decryptor.InvariantNoiseBudget(encrypted1));
            Console.WriteLine("Noise budget in encryption of {0}: {1} bits", value2, decryptor.InvariantNoiseBudget(encrypted2));
            Console.WriteLine("Noise budget in sum: {0} bits", decryptor.InvariantNoiseBudget(encryptedSum));
            Console.WriteLine("Noise budget in product: {0} bits", decryptor.InvariantNoiseBudget(encryptedProduct));
        }


        private void PrintBigPolyArray(StringBuilder stBlder, String message, BigPolyArray array)
        {
            stBlder.AppendLine(message);
            for (int i = 0; i < array.Size; i++)
            {
                stBlder.Append(array[i] + "-");
            }
        }
    }
}
